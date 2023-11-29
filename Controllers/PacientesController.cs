using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]

public class PacientesController:ControllerBase{
    /*La cadena de conexion esta en Program.cs*/
    private readonly MySqlConnection _conexion;

    public PacientesController(MySqlConnection conexion){
        _conexion = conexion;
    }
    /*CREAR UN MÉTODO GET QUE LISTE TODOS LOS PACIENTES QUE ATIENDE LA CLÍNICA (02
    puntos)*/
  [HttpGet]
  [Route("Lista de Pacientes")]
    public ActionResult<IEnumerable<Pacientes>> ObtenerTodosLosPacientes()
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*aqui se crea el comando SQL para selecionar a los Pacientes */
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM Pacientes", _conexion))
            {
                /*aqui se ejecuta el comando de arriba*/
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    /*aqui se crea una lista para almacenar los Pacientes*/
                    List<Pacientes> pacientesList = new List<Pacientes>();
                    /*este while crea a los Pacientes*/
                    while (reader.Read())
                    {
                        /*crea al Paciente a partir de los datos leidos*/
                        Pacientes paciente = new Pacientes
                        {
                            idPaciente = reader.GetInt32("idPaciente"),
                            NombrePac = reader["NombrePac"].ToString(),
                            ApellidoPac = reader["ApellidoPac"].ToString(),
                            RunPac = reader["RunPac"].ToString(),
                            Nacionalidad = reader["Nacionalidad"].ToString(),
                            Visa = reader["Visa"].ToString(),
                            Genero = reader["Genero"].ToString(),
                            SintomaPac = reader["SintomaPac"].ToString(),
                            M_idMedico = reader.GetInt32("M_idMedico")
                        };
                        /*aqui se agrean a los pacientes a la lista*/
                        pacientesList.Add(paciente);
                    }
                    /*aqui se devuelve la lista como respuesta*/
                    return StatusCode(200,pacientesList);
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
    /*CREAR UN MÉTODO GET QUE LISTE TODOS LOS DATOS DE UN PACIENTE EN PARTICULAR (02
    puntos)*/
    [HttpGet]
    [Route("Paciente por {id}")]
    public ActionResult<Pacientes> ObtenerPacientePorId(int id)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*aqui se crea el comando SQL para selecionar a los Pacientes con WHERE para diferenciar por id */
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM Pacientes WHERE idPaciente = @idPaciente", _conexion))
            {   
                /*se agrega el id como parametro*/
                command.Parameters.AddWithValue("@idPaciente", id);
                /*aqui se ejecuta el comando de arriba*/
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    /*este if verifica si existe el paciente con la id entregada*/
                    if (reader.Read())
                    {
                        /*crea al paciente a partir de los datos leidos*/
                        Pacientes paciente = new Pacientes
                        {
                            idPaciente = reader.GetInt32("idPaciente"),
                            NombrePac = reader["NombrePac"].ToString(),
                            ApellidoPac = reader["ApellidoPac"].ToString(),
                            RunPac = reader["RunPac"].ToString(),
                            Nacionalidad = reader["Nacionalidad"].ToString(),
                            Visa = reader["Visa"].ToString(),
                            Genero = reader["Genero"].ToString(),
                            SintomaPac = reader["SintomaPac"].ToString(),
                            M_idMedico = reader.GetInt32("M_idMedico")
                        };
                        /*se devuelve el medico solicitado como respuesta exitosa*/
                        return StatusCode(200,paciente);
                    }
                    else
                    {
                        /*error para cuando no se encuentra el paciente*/
                        return StatusCode(404,$"No se encontró el paciente con ID {id}");
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
    /*CREAR UN MÉTODO POST QUE PERMITA CREAR Y GUARDAR UN NUEVO PACIENTE (CON TODOS
    LOS DATOS PEDIDOS) (02 puntos)*/
    [HttpPost]
    [Route("Crear Paciente")]
    public ActionResult<Pacientes> CrearPaciente([FromBody] Pacientes nuevoPaciente)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*Creacion del comando sql para el INSERT en la tabla de Pacientes*/
            using (MySqlCommand command = new MySqlCommand(
                "INSERT INTO Pacientes (NombrePac, ApellidoPac, RunPac, Nacionalidad, Visa, Genero, SintomaPac, M_idMedico) " +
                "VALUES (@NombrePac, @ApellidoPac, @RunPac, @Nacionalidad, @Visa, @Genero, @SintomaPac, @M_idMedico)",
                _conexion))
            {
                /*aqui se agregan parametros al comando con los valores del Paciente*/
                command.Parameters.AddWithValue("@NombrePac", nuevoPaciente.NombrePac);
                command.Parameters.AddWithValue("@ApellidoPac", nuevoPaciente.ApellidoPac);
                command.Parameters.AddWithValue("@RunPac", nuevoPaciente.RunPac);
                command.Parameters.AddWithValue("@Nacionalidad", nuevoPaciente.Nacionalidad);
                command.Parameters.AddWithValue("@Visa", nuevoPaciente.Visa);
                command.Parameters.AddWithValue("@Genero", nuevoPaciente.Genero);
                command.Parameters.AddWithValue("@SintomaPac", nuevoPaciente.SintomaPac);
                command.Parameters.AddWithValue("@M_idMedico",nuevoPaciente.M_idMedico);
                /*aqui se ejecuta el comando de insercion*/
                command.ExecuteNonQuery();
                /*Aqui se devuelve al nuevo paciente, no ocupe el statuscode(200) para facilitar la creacion del codigo*/
                return CreatedAtAction(nameof(ObtenerPacientePorId), new { id = nuevoPaciente.idPaciente }, nuevoPaciente);
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
    /*CREAR UN MÉTODO PUT QUE PERMITA EDITAR Y GUARDAR CAMBIOS A UN PACIENTE
    SELECCIONADO (CON TODOS LOS DATOS PEDIDOS) (03 puntos)*/
    [HttpPut]
    [Route("Actualizar Paciente {id}")]
    public ActionResult<Pacientes> ActualizarPaciente(int id, [FromBody] Pacientes pacienteActualizado)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
             /*creacion de comando sql para hacer UPDATE con un WHERE por id*/
            using (MySqlCommand command = new MySqlCommand(
                "UPDATE Pacientes SET NombrePac = @NombrePac, ApellidoPac = @ApellidoPac, RunPac = @RunPac, " +
                "Nacionalidad = @Nacionalidad, Visa = @Visa, Genero = @Genero, SintomaPac = @SintomaPac, M_idMedico = @M_idMedico " +
                "WHERE idPaciente = @idPaciente",
                _conexion))
            {
                /*aqui se agregan parametros al comando con los valores del paciente*/
                command.Parameters.AddWithValue("@idPaciente", id);
                command.Parameters.AddWithValue("@NombrePac", pacienteActualizado.NombrePac);
                command.Parameters.AddWithValue("@ApellidoPac", pacienteActualizado.ApellidoPac);
                command.Parameters.AddWithValue("@RunPac", pacienteActualizado.RunPac);
                command.Parameters.AddWithValue("@Nacionalidad", pacienteActualizado.Nacionalidad);
                command.Parameters.AddWithValue("@Visa", pacienteActualizado.Visa);
                command.Parameters.AddWithValue("@Genero", pacienteActualizado.Genero);
                command.Parameters.AddWithValue("@SintomaPac", pacienteActualizado.SintomaPac);
                command.Parameters.AddWithValue("@M_idMedico",pacienteActualizado.M_idMedico);
                /*ejecucion del comando y la obtencion de filas modificadas*/
                int rowsAffected = command.ExecuteNonQuery();
                /*esto verifica la actualizacion del paciente*/
                if (rowsAffected > 0)
                {
                    /*esto devulve al medico actualizdo*/
                    return StatusCode(200,pacienteActualizado);
                }
                else
                {
                    /*error para cuando no se encuentra el medico*/
                    return StatusCode(404,$"No se encontró el paciente con ID {id}");
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
    [Route("Borrar Paciente {id}")]
    public ActionResult EliminarPaciente(int id)
    {
        try
        {
            /*Esta linea habre la conexion a la base de datos*/
            _conexion.Open();
            /*esto crea el comando DELETE con WHERE por id*/
            using (MySqlCommand command = new MySqlCommand("DELETE FROM Pacientes WHERE idPaciente = @idPaciente", _conexion))
            {
                /*se agrega el id como parametro*/
                command.Parameters.AddWithValue("@idPaciente", id);
                /*ejecucion del comando y la obtencion de filas modificadas*/
                int rowsAffected = command.ExecuteNonQuery();
                 /*esto verifica la eliminacion del paciente*/
                if (rowsAffected > 0)
                {   
                    /*esto devuleve un codigo 204 si la eliminacion es exitosa*/
                    return StatusCode(204,"Paciente Eliminado correctamente");
                }
                else
                {
                    /*error para cuando no se encuentra el medico*/
                    return StatusCode(404,$"No se encontró el paciente con ID {id}");
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