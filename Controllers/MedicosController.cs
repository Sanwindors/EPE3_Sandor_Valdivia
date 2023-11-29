using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]

public class MedicoController : ControllerBase{
    /*La cadena de conexion esta en Program.cs*/
    private readonly MySqlConnection _conexion;
    public MedicoController(MySqlConnection conexion){
        _conexion = conexion;
    }
/*CREAR UN MÉTODO GET QUE LISTE TODOS LOS DATOS DE LOS MÉDICOS CON QUE CUENTA LA
CLÍNICA (02 puntos)*/
[HttpGet]
[Route("Listar Medico")]
public ActionResult<IEnumerable<Medicos>> ObtenerTodosLosMedicos()
{
    try
    {   
        /*Esta linea habre la conexion a la base de datos*/
        _conexion.Open();
        /*aqui se crea el comando SQL para selecionar a los medicos */
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM Medicos", _conexion))
        {   
            /*aqui se ejecuta el comando de arriba*/
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                /*aqui se crea una lista para almacenar los medicos*/
                List<Medicos> medicosList = new List<Medicos>();
                /*este while crea a los medicos*/
                while (reader.Read())
                {
                    /*crea al medico a partir de los datos leidos*/
                    Medicos medico = new Medicos
                    {
                        idMedico = Convert.ToInt32(reader["IdMedico"]),
                        NombreMed = reader["NombreMed"].ToString(),
                        ApellidoMed = reader["ApellidoMed"].ToString(),
                        RunMed = reader["RunMed"].ToString(),
                        EunaCom = reader["EunaCom"].ToString(),
                        NacionalidadMed = reader["NacionalidadMed"].ToString(),
                        Especialidad = reader["Especialidad"].ToString(),
                        TarifaHr = Convert.ToInt32(reader["TarifaHr"])
                    };

                    // if para manejar la conversion de TimeOnly a Time de la base de datos para que no haya problemas
                    if (reader["Horarios"] != DBNull.Value)
                    {
                        TimeSpan timeSpan = (TimeSpan)reader["Horarios"];
                        medico.Horarios = new TimeOnly(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    }
                    /*aqui se agrean a los medicos a la lista*/
                    medicosList.Add(medico);
                }
                /*aqui se devuelve la lista como respuesta*/
                return StatusCode(200,medicosList);
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
/*CREAR UN MÉTODO GET QUE LISTE TODOS LOS DATOS DE UN MÉDICO EN PARTICULAR (02
puntos)*/
[HttpGet]
[Route("Medico {id}")]
public ActionResult<Medicos> ObtenerMedicoPorId(int id)
{
    try
    {
       /*Esta linea habre la conexion a la base de datos*/
        _conexion.Open();
        /*aqui se crea el comando SQL para selecionar a los medicos con WHERE para diferenciar por id */
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM Medicos WHERE IdMedico = @IdMedico", _conexion))
        {
            /*se agrega el id como parametro*/
            command.Parameters.AddWithValue("@IdMedico", id);
            /*aqui se ejecuta el comando de arriba*/
            using (MySqlDataReader reader = command.ExecuteReader())
            {   
                /*este if verifica si existe el medico con la id entregada*/
                if (reader.Read())
                {
                    /*crea al medico a partir de los datos leidos*/
                    Medicos medico = new Medicos
                    {
                        idMedico = Convert.ToInt32(reader["IdMedico"]),
                        NombreMed = reader["NombreMed"].ToString(),
                        ApellidoMed = reader["ApellidoMed"].ToString(),
                        RunMed = reader["RunMed"].ToString(),
                        EunaCom = reader["EunaCom"].ToString(),
                        NacionalidadMed = reader["NacionalidadMed"].ToString(),
                        Especialidad = reader["Especialidad"].ToString(),
                        TarifaHr = Convert.ToInt32(reader["TarifaHr"])
                    };

                    // if para manejar la conversion de TimeOnly a Time de la base de datos para que no haya problemas
                    if (reader["Horarios"] != DBNull.Value)
                    {
                        TimeSpan timeSpan = (TimeSpan)reader["Horarios"];
                        medico.Horarios = new TimeOnly(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                    }
                    /*se devuelve el medico solicitado como respuesta exitosa*/
                    return StatusCode(200,medico);
                }
                else
                {   
                    /*error para cuando no se encuentra el medico*/
                    return StatusCode(404,$"No se encontró el médico con ID {id}");
                }
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
/*CREAR UN MÉTODO POST QUE PERMITA CREAR Y GUARDAR UN NUEVO MEDICO (CON TODOS
LOS DATOS PEDIDOS) (02 puntos)*/
    [HttpPost]
    [Route("Crear Medico")]
    public ActionResult<Medicos> CrearMedico([FromBody] Medicos nuevoMedico)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*Creacion del comando sql para el INSERT en la tabla de Medicos*/
            using (MySqlCommand command = new MySqlCommand(
                "INSERT INTO Medicos (NombreMed, ApellidoMed, RunMed, EunaCom, NacionalidadMed, Especialidad, Horarios, TarifaHr) " +
                "VALUES (@NombreMed, @ApellidoMed, @RunMed, @EunaCom, @NacionalidadMed, @Especialidad, @Horarios, @TarifaHr)",
                _conexion))
            {
                /*aqui se agregan parametros al comando con los valores del medico*/
                command.Parameters.AddWithValue("@NombreMed", nuevoMedico.NombreMed);
                command.Parameters.AddWithValue("@ApellidoMed", nuevoMedico.ApellidoMed);
                command.Parameters.AddWithValue("@RunMed", nuevoMedico.RunMed);
                command.Parameters.AddWithValue("@EunaCom", nuevoMedico.EunaCom);
                command.Parameters.AddWithValue("@NacionalidadMed", nuevoMedico.NacionalidadMed);
                command.Parameters.AddWithValue("@Especialidad", nuevoMedico.Especialidad);
                command.Parameters.AddWithValue("@Horarios", nuevoMedico.Horarios);
                command.Parameters.AddWithValue("@TarifaHr", nuevoMedico.TarifaHr);
                /*aqui se ejecuta el comando de insercion*/
                command.ExecuteNonQuery();
                /*Aqui se devuelve al nuevo medico, no ocupe el statuscode(200) para facilitar la creacion del codigo*/
                return CreatedAtAction(nameof(ObtenerMedicoPorId), new { id = nuevoMedico.idMedico }, nuevoMedico);
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
/*CREAR UN MÉTODO PUT QUE PERMITA EDITAR Y GUARDAR CAMBIOS A UN MEDICO
SELECCIONADO (CON TODOS LOS DATOS PEDIDOS) (03 puntos)*/
[HttpPut]
[Route("Actualizar Medico {id}")]
public ActionResult<Medicos> ActualizarMedico(int id, [FromBody] Medicos medicoActualizado)
{
    try
    {
        /*Esta linea habre la conexion a la base de datos*/
        _conexion.Open();
        /*creacion de comando sql para hacer UPDATE con un WHERE por id*/
        using (MySqlCommand command = new MySqlCommand(
            "UPDATE Medicos SET NombreMed = @NombreMed, ApellidoMed = @ApellidoMed, RunMed = @RunMed, " +
            "EunaCom = @EunaCom, NacionalidadMed = @NacionalidadMed, Especialidad = @Especialidad, " +
            "Horarios = @Horarios, TarifaHr = @TarifaHr WHERE IdMedico = @IdMedico",
            _conexion))
        {   
            /*aqui se agregan parametros al comando con los valores del medico*/
            command.Parameters.AddWithValue("@IdMedico", id);
            command.Parameters.AddWithValue("@NombreMed", medicoActualizado.NombreMed);
            command.Parameters.AddWithValue("@ApellidoMed", medicoActualizado.ApellidoMed);
            command.Parameters.AddWithValue("@RunMed", medicoActualizado.RunMed);
            command.Parameters.AddWithValue("@EunaCom", medicoActualizado.EunaCom);
            command.Parameters.AddWithValue("@NacionalidadMed", medicoActualizado.NacionalidadMed);
            command.Parameters.AddWithValue("@Especialidad", medicoActualizado.Especialidad);
            command.Parameters.AddWithValue("@Horarios", medicoActualizado.Horarios);
            command.Parameters.AddWithValue("@TarifaHr", medicoActualizado.TarifaHr);

            /*ejecucion del comando y la obtencion de filas modificadas*/
            int rowsAffected = command.ExecuteNonQuery();
            /*esto verifica la actualizacion del medico*/
            if (rowsAffected > 0)
            {
                /*esto devulve al medico actualizdo*/
                return StatusCode(200,medicoActualizado);
            }
            else
            {
                /*error para cuando no se encuentra el medico*/
                return StatusCode(404,$"No se encontró el médico con ID {id}");
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
/*CREAR UN MÉTODO DELETE QUE PERMITA ELIMINAR UN PACIENTE SELECCIONADO (02 puntos)*/
[HttpDelete]
[Route("Borrar Medico {id}")]
public ActionResult EliminarMedico(int id)
{
    try
    {
        /*Esta linea habre la conexion a la base de datos*/
        _conexion.Open();
        /*esto crea el comando DELETE con WHERE por id*/
        using (MySqlCommand command = new MySqlCommand("DELETE FROM Medicos WHERE IdMedico = @IdMedico", _conexion))
        {
            /*se agrega el id como parametro*/
            command.Parameters.AddWithValue("@IdMedico", id);
            /*ejecucion del comando y la obtencion de filas modificadas*/
            int rowsAffected = command.ExecuteNonQuery();
            /*esto verifica la eliminacion del medico*/
            if (rowsAffected > 0)
            {   
                /*esto devuleve un codigo 204 si la eliminacion es exitosa*/
                return StatusCode(204,"Medico Eliminado correctamente");
            }
            else
            {
                 /*error para cuando no se encuentra el medico*/
                return StatusCode(404,$"No se encontró el médico con ID {id}");
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
}