using SQLite;

namespace MauiApp4
{public partial class SuccessPage : ContentPage

{

    public class Contacto

    {

        [PrimaryKey, AutoIncrement]

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string CorreoElectronico { get; set; }

    }



    public class DatabaseManager

    {

        readonly SQLiteConnection database;



        public DatabaseManager(string dbPath)

        {

            database = new SQLiteConnection(dbPath);

            database.CreateTable<Contacto>();

        }



        public List<Contacto> ObtenerContactos()

        {

            return database.Table<Contacto>().ToList();

        }



        public Contacto ObtenerContactoPorNombre(string nombre)

        {

            return database.Table<Contacto>().FirstOrDefault(c => c.Nombre == nombre);

        }



        public int GuardarContacto(Contacto contacto)

        {

            if (contacto.Id != 0)

            {

                return database.Update(contacto);

            }

            else

            {

                return database.Insert(contacto);

            }

        }



        public int EliminarContacto(Contacto contacto)

        {

            return database.Delete(contacto);

        }

    }



    DatabaseManager dbManager;

    Contacto contactoEncontrado;

    public SuccessPage()

    {

        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contacts.db");

        dbManager = new DatabaseManager(dbPath);

    }

    private void Guardar_Clicked(object sender, EventArgs e)

    {

        if (contactoEncontrado != null)

        {

            // Modificar el contacto encontrado

            contactoEncontrado.Direccion = direccionEntry.Text;

            contactoEncontrado.Telefono = telefonoEntry.Text;

            contactoEncontrado.CorreoElectronico = correoEntry.Text;



            dbManager.GuardarContacto(contactoEncontrado);

            DisplayAlert("Modificar", "Registro Modificado!", "OK");

        }

        else

        {

            // Agregar un nuevo contacto

            var contacto = new Contacto

            {

                Nombre = nombreEntry.Text,

                Direccion = direccionEntry.Text,

                Telefono = telefonoEntry.Text,

                CorreoElectronico = correoEntry.Text

            };



            dbManager.GuardarContacto(contacto);

            DisplayAlert("Agregar", "Registro Agregado!", "OK");

        }

    }



    private void Buscar_Clicked(object sender, EventArgs e)

    {

        string nombre = nombreEntry.Text;

        contactoEncontrado = dbManager.ObtenerContactoPorNombre(nombre);



        if (contactoEncontrado != null)

        {

            direccionEntry.Text = contactoEncontrado.Direccion;

            telefonoEntry.Text = contactoEncontrado.Telefono;

            correoEntry.Text = contactoEncontrado.CorreoElectronico;

            ModificarButton.IsEnabled = true;

        }

        else

        {



            // Mostrar un mensaje indicando que el contacto no fue encontrado.

            ModificarButton.IsEnabled = false;

            DisplayAlert("Buscar", "Registro no encontrado!", "OK");

        }

    }



    private void Eliminar_Clicked(object sender, EventArgs e)

    {

        if (contactoEncontrado != null)

        {

            dbManager.EliminarContacto(contactoEncontrado);

            LimpiarCampos();

            DisplayAlert("Eliminar", "Registro eliminado!", "OK");

        }

        else

        {

            // Mostrar un mensaje indicando que el contacto no fue encontrado.

            DisplayAlert("Eliminar", "Registro no encontrado!", "OK");

        }

    }



    private void Modificar_Clicked(object sender, EventArgs e)

    {

        // Habilitar campos para permitir modificaciones

        direccionEntry.IsEnabled = true;

        telefonoEntry.IsEnabled = true;

        correoEntry.IsEnabled = true;

        GuardarButton.IsEnabled = true;

    }



    private void LimpiarCampos()

    {

        nombreEntry.Text = string.Empty;

        direccionEntry.Text = string.Empty;

        telefonoEntry.Text = string.Empty;

        correoEntry.Text = string.Empty;

        contactoEncontrado = null;

        ModificarButton.IsEnabled = false;

        direccionEntry.IsEnabled = false;

        telefonoEntry.IsEnabled = false;

        correoEntry.IsEnabled = false;

        GuardarButton.IsEnabled = false;

    }

}
}