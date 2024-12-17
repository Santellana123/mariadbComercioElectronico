using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

class Program
{
    private static string connectionString = "Server=localhost;Database=comercio_electronico;User ID=admin;Password=passwordadmin;";

    private static string usuarioLogueado;
    private static bool esAdmin;

    static void Main(string[] args)
    {
        if (Login())
        {
            Console.WriteLine("Login exitoso.");
            MostrarMenu();
        }
        else
        {
            Console.WriteLine("Credenciales incorrectas. Saliendo del programa...");
        }
    }

    static bool Login()
    {
        Console.Write("Nombre de usuario: ");
        string username = Console.ReadLine();

        Console.Write("Contraseña: ");
        string password = Console.ReadLine();

        // Encriptar la contraseña ingresada
        string hashedPassword = EncriptarSHA256(password);

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Usuario WHERE Nombre = @username AND Contrasena = @password";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                    object result = command.ExecuteScalar();
                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        usuarioLogueado = username;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Credenciales incorrectas.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                return false;
            }
        }
    }

    static void EjecutarComando(string query, Action<MySqlCommand> configurarComando)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    configurarComando(command);
                    int filasAfectadas = command.ExecuteNonQuery();
                    Console.WriteLine($"Operación completada. Filas afectadas: {filasAfectadas}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar el comando: {ex.Message}");
            }
        }
    }

    // Los métodos MostrarMenu, CrudUsuarios, CrudProductos, CrearUsuario, LeerUsuarios, etc.
    // Permanecen iguales excepto por los cambios en las conexiones y comandos, reemplazando Sql por MySql
    // en las clases utilizadas.

    static string EncriptarSHA256(string texto)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
            StringBuilder resultado = new StringBuilder();
            foreach (byte b in bytes)
            {
                resultado.Append(b.ToString("x2"));
            }
            return resultado.ToString();
        }
    }
}
