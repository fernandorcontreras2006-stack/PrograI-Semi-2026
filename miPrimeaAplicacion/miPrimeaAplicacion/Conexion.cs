using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data; //Esta libreria es para usar bases de datos
using System.Data.SqlClient; //Esta libreria permite usar SQLServer

namespace miPrimeaAplicacion {
    internal class Conexion {
        //Definir los miembros, atributos y metodos de la clase
        public SqlConnection objConexion = new SqlConnection(); //Conectarme a la BD
        public SqlCommand objComando = new SqlCommand(); //Ejecutar consultas (Insert, update, delete, select) SQL en la BD
        public SqlDataAdapter objDataAdapter = new SqlDataAdapter();//un puente entre la BD y la aplicacion.
        DataSet objDs = new DataSet(); //Representa una copia en memoria de la arquitectura de la BD

        public Conexion() {//Constructor e inicializador de los miembros de la clase
            String cadenaCoenxion = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\db_academica.mdf;Integrated Security=True";
            objConexion.ConnectionString = cadenaCoenxion;
            objConexion.Open(); //abrir la BD
            inicializarTablaUsuarios();
        }

        // Crea la tabla solicitada si todavía no existe y agrega tres registros de ejemplo.
        private void inicializarTablaUsuarios()
        {
            string sql = @"
IF OBJECT_ID('dbo.usuarios_usss032426', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.usuarios_usss032426 (
        idUsuario INT IDENTITY(1,1) PRIMARY KEY,
        usuario VARCHAR(35) NOT NULL UNIQUE,
        clave VARCHAR(35) NOT NULL,
        nombre VARCHAR(65) NOT NULL,
        direccion VARCHAR(85) NOT NULL,
        telefono VARCHAR(9) NOT NULL
    );
END;
IF NOT EXISTS (SELECT 1 FROM dbo.usuarios_usss032426)
BEGIN
    INSERT INTO dbo.usuarios_usss032426 (usuario, clave, nombre, direccion, telefono)
    VALUES ('fer1234', 'Clave123', 'Fernando Perez', 'San Miguel', '76543210'),
           ('maria123', 'Maria456', 'Maria Lopez', 'Usulutan', '76543211'),
           ('carlos1', 'Carlos789', 'Carlos Gomez', 'San Salvador', '76543212');
END;";
            using (SqlCommand cmd = new SqlCommand(sql, objConexion))
                cmd.ExecuteNonQuery();
        }
        public DataSet obtenerDatos() {
            objDs.Clear();//Limpiar el datase.
            objComando.Connection = objConexion;//Establcer la conexion para ejecutar consultas a la BD

            objDataAdapter.SelectCommand = objComando;

            objComando.CommandText = "SELECT * FROM alumnos";
            objDataAdapter.Fill(objDs, "alumnos"); //tomamos los datos de la BD y llenamos el ds

            objComando.CommandText = "SELECT * FROM materias";
            objDataAdapter.Fill(objDs, "materias");

            objComando.CommandText = "SELECT * FROM usuarios_usss032426";
            objDataAdapter.Fill(objDs, "usuarios_usss032426");

            return objDs;
        }
        public string administrarDatosAlumnos(String[] datos, String accion)
        {
            String sql = "";
            if (accion == "nuevo"){
                sql = "INSERT INTO alumnos(codigo,nombre,direccion,telefono,email) VALUES ('" + datos[1] + "', '" + datos[2] + "', '" + datos[3] + "', '" + datos[4] + "', '" + datos[5] + "')";
            }else if (accion == "modificar"){
                sql = "UPDATE alumnos SET codigo='" + datos[1] + "', nombre='" + datos[2] + "', direccion='" + datos[3] + "', telefono='" + datos[4] + "', email='" + datos[5] + "' WHERE idAlumno='" + datos[0] + "'";
            }else if (accion == "eliminar"){
                sql = "DELETE FROM alumnos WHERE idAlumno='" + datos[0] + "'";
            }
            return ejecutarSQL(sql);
        }
        public string administrarDatosMaterias(String[] datos, String accion){
            String sql = "";
            if (accion == "nuevo"){
                sql = "INSERT INTO materias(codigo,nombre,uv) VALUES ('" + datos[1] + "', '" + datos[2] + "', '" + datos[3] + "')";
            }else if (accion == "modificar"){
                sql = "UPDATE materias SET codigo='" + datos[1] + "', nombre='" + datos[2] + "', uv='" + datos[3] + "' WHERE idMateria='" + datos[0] + "'";
            }else if (accion == "eliminar"){
                sql = "DELETE FROM materias WHERE idMateria='" + datos[0] + "'";
            }
            return ejecutarSQL(sql);
        }
        // Método procesar solicitado en el ejercicio. Ejecuta SQL con parámetros opcionales.
        public string procesar(string sql, SqlParameter[] parametros = null)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, objConexion))
                {
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);
                    return cmd.ExecuteNonQuery().ToString();
                }
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        // Mantenimiento de usuarios: accion debe ser nuevo, modificar o eliminar.
        // datos: [idUsuario, usuario, clave, nombre, direccion, telefono]
        public string mantenimiento_usuarios_usss032426(string[] datos, string accion)
        {
            string sql;
            SqlParameter[] parametros;

            if (accion == "nuevo")
            {
                sql = @"INSERT INTO usuarios_usss032426
                        (usuario, clave, nombre, direccion, telefono)
                        VALUES (@usuario, @clave, @nombre, @direccion, @telefono)";
                parametros = new SqlParameter[] {
                    new SqlParameter("@usuario", SqlDbType.VarChar, 35) { Value = datos[1] },
                    new SqlParameter("@clave", SqlDbType.VarChar, 35) { Value = datos[2] },
                    new SqlParameter("@nombre", SqlDbType.VarChar, 65) { Value = datos[3] },
                    new SqlParameter("@direccion", SqlDbType.VarChar, 85) { Value = datos[4] },
                    new SqlParameter("@telefono", SqlDbType.VarChar, 9) { Value = datos[5] }
                };
            }
            else if (accion == "modificar")
            {
                sql = @"UPDATE usuarios_usss032426 SET usuario=@usuario, clave=@clave,
                        nombre=@nombre, direccion=@direccion, telefono=@telefono
                        WHERE idUsuario=@idUsuario";
                parametros = new SqlParameter[] {
                    new SqlParameter("@idUsuario", SqlDbType.Int) { Value = Convert.ToInt32(datos[0]) },
                    new SqlParameter("@usuario", SqlDbType.VarChar, 35) { Value = datos[1] },
                    new SqlParameter("@clave", SqlDbType.VarChar, 35) { Value = datos[2] },
                    new SqlParameter("@nombre", SqlDbType.VarChar, 65) { Value = datos[3] },
                    new SqlParameter("@direccion", SqlDbType.VarChar, 85) { Value = datos[4] },
                    new SqlParameter("@telefono", SqlDbType.VarChar, 9) { Value = datos[5] }
                };
            }
            else if (accion == "eliminar")
            {
                sql = "DELETE FROM usuarios_usss032426 WHERE idUsuario=@idUsuario";
                parametros = new SqlParameter[] {
                    new SqlParameter("@idUsuario", SqlDbType.Int) { Value = Convert.ToInt32(datos[0]) }
                };
            }
            else
            {
                return "Acción no válida.";
            }

            // El método de mantenimiento llama a procesar, como pide el enunciado.
            return procesar(sql, parametros);
        }

        public bool validarUsuario(string usuario, string clave)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM usuarios_usss032426 WHERE usuario=@usuario AND clave=@clave", objConexion))
            {
                cmd.Parameters.Add("@usuario", SqlDbType.VarChar, 35).Value = usuario;
                cmd.Parameters.Add("@clave", SqlDbType.VarChar, 35).Value = clave;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public String ejecutarSQL(String sql)
        {
            try
            {
                objComando.Connection = objConexion;
                objComando.CommandText = sql;
                return objComando.ExecuteNonQuery().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
