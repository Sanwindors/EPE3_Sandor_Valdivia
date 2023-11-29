using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]

public class ReservasController:ControllerBase{
    /*La cadena de conexion esta en Program.cs*/
    private readonly MySqlConnection _conexion;

    public ReservasController(MySqlConnection conexion){
        _conexion = conexion;
    }
    /*CREAR UN MÉTODO GET QUE LISTE TODAS LAS RESERVAS HECHAS EN LA CLÍNICA (02 puntos)*/
    [HttpGet]
    [Route("Listar Reservas")]
    public ActionResult<IEnumerable<Reservas>> ObtenerTodasLasReservas()
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
             /*aqui se crea el comando SQL para selecionar las reservas */
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM Reservas", _conexion))
            {   
                /*aqui se ejecuta el comando de arriba*/
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    /*aqui se crea una lista para almacenar las reservas*/
                    List<Reservas> reservasList = new List<Reservas>();
                    /*este while crea las reservas*/
                    while (reader.Read())
                    {
                        /*crea las reservas a partir de los datos leidos*/
                        Reservas reserva = new Reservas
                        {
                            idReserva = reader.GetInt32("idReserva"),
                            Especialidad = reader["Especialidad"].ToString(),
                            DiaReserva = DateTimeOffset.Parse(reader["DiaReserva"].ToString()),
                            P_idPaciente = reader.GetInt32("P_idPaciente")
                        };
                        /*aqui se agrean las reservas a la lista*/
                        reservasList.Add(reserva);
                    }
                    /*aqui se devuelve la lista como respuesta*/
                    return StatusCode(200,reservasList);
                }
            }
        }
        catch (MySqlException ex)
        {
            /*codigo de error para excepciones de MySql y codigo de error interno*/
            return StatusCode(500, $"Error: {ex.Message}");
        }
        finally
        {
            /*Cierre de la conexion con la base de datos*/
            _conexion.Close();
        }
    }
    /*CREAR UN MÉTODO POST QUE PERMITA CREAR Y GUARDAR UNA NUEVA RESERVA MÉDICA (CON
    TODOS LOS DATOS PEDIDOS) (02 puntos)*/
    [HttpPost]
    [Route("Crear Reserva")]
    public ActionResult<Reservas> CrearReserva([FromBody] Reservas nuevaReserva)
    {
        try
        {
             /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
             /*Creacion del comando sql para el INSERT en la tabla de Reservas*/
            using (MySqlCommand command = new MySqlCommand(
                "INSERT INTO Reservas (Especialidad, DiaReserva, P_idPaciente) " +
                "VALUES (@Especialidad, @DiaReserva, @P_idPaciente)",
                _conexion))
            {
                /*aqui se agregan parametros al comando con los valores de la reserva*/
                command.Parameters.AddWithValue("@Especialidad", nuevaReserva.Especialidad);
                command.Parameters.AddWithValue("@DiaReserva", nuevaReserva.DiaReserva);
                command.Parameters.AddWithValue("@P_idPaciente",nuevaReserva.P_idPaciente);
                /*aqui se ejecuta el comando de insercion*/
                command.ExecuteNonQuery();
                /*Aqui se devuelve la nueva reserva, no ocupe el statuscode(200) para facilitar la creacion del codigo*/
                return CreatedAtAction(nameof(ObtenerTodasLasReservas), new { id = nuevaReserva.idReserva }, nuevaReserva);
            }
        }
        catch (MySqlException ex)
        {
            /*codigo de error para excepciones de MySql y codigo de error interno*/
            return StatusCode(500, $"Error: {ex.Message}");
        }
        finally
        {
             /*Cierre de la conexion con la base de datos*/
            _conexion.Close();
        }
    }

    /*CREAR UN MÉTODO PUT QUE PERMITA EDITAR Y GUARDAR CAMBIOS A UNA RESERVA MÉDICA
    SELECCIONADA (CON TODOS LOS DATOS PEDIDOS) (03 puntos)*/
    [HttpPut]
    [Route("Actualizar Reserva {id}")]
    public ActionResult<Reservas> ActualizarReserva(int id, [FromBody] Reservas reservaActualizada)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*creacion de comando sql para hacer UPDATE con un WHERE por id*/
            using (MySqlCommand command = new MySqlCommand(
                "UPDATE Reservas SET Especialidad = @Especialidad, DiaReserva = @DiaReserva " +
                "WHERE idReserva = @idReserva",
                _conexion))
            {
                 /*aqui se agregan parametros al comando con los valores de la reserva*/
                command.Parameters.AddWithValue("@idReserva", id);
                command.Parameters.AddWithValue("@Especialidad", reservaActualizada.Especialidad);
                command.Parameters.AddWithValue("@DiaReserva", reservaActualizada.DiaReserva);
                command.Parameters.AddWithValue("@P_idPaciente",reservaActualizada.P_idPaciente);
                /*ejecucion del comando y la obtencion de filas modificadas*/
                int rowsAffected = command.ExecuteNonQuery();
                /*esto verifica la actualizacion de la reserva*/
                if (rowsAffected > 0)
                {
                    /*esto devulve la reserva actualizada*/
                    return StatusCode(200,reservaActualizada);
                }
                else
                {
                     /*error para cuando no se encuentra la reserva*/
                    return StatusCode(404,$"No se encontró la reserva con ID {id}");
                }
            }
        }
        catch (MySqlException ex)
        {
            /*codigo de error para excepciones de MySql y codigo de error interno*/
            return StatusCode(500, $"Error: {ex.Message}");
        }
        finally
        {
            /*Cierre de la conexion con la base de datos*/
            _conexion.Close();
        }
    }
    /*CREAR UN MÉTODO DELETE QUE PERMITA ELIMINAR UNA RESERVA SELECCIONADA (02 puntos)*/
    [HttpDelete]
    [Route("Borrar Reserva {id}")]
    public async Task<IActionResult> DeleteReserva(int id)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            await _conexion.OpenAsync();
            /*esto crea el comando DELETE con WHERE por id*/
            using (var cmd = _conexion.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Reservas WHERE idReserva = @id";
                cmd.Parameters.AddWithValue("@id", id);
                /*ejecucion del comando y la obtencion de filas modificadas*/
                var filasAfectadas = await cmd.ExecuteNonQueryAsync();

                if (filasAfectadas > 0)
                {
                     /*esto devuleve un codigo 204 si la eliminacion es exitosa*/
                    return StatusCode(204,"Reserva eliminada correctamente.");
                }
                else
                {
                    /*error para cuando no se encuentra el medico*/
                    return StatusCode(404,"No se encontró la reserva con el ID proporcionado.");
                }
            }
        }
        catch (Exception ex)
        {
            /*codigo de error para excepciones de MySql y codigo de error interno*/
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
        finally
        {
            /*Cierre de la conexion con la base de datos*/
            _conexion.Close();
        }
    }
}