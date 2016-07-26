using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP
    /// </summary>
    public class GameModel : TgcExample
    {
        //Caja que se muestra en el ejemplo
        private TgcBox Box { get; set; }

        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }

        /// <summary>
        /// Constructor del juego
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas
            var d3dDevice = D3DDevice.Instance.Device;

            //Piso la camara que viene por defecto con la que yo quiero usar.
            //Configurar centro al que se mira y distancia desde la que se mira
            Camara = new TgcRotationalCamera(new Vector3(), 50f, Input);

            //Textura de la carperta Media
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada en (0, 0, 0), dimensiones (5, 10, 5) y la textura como color.
            var center = new Vector3(0, 0, 0);
            var size = new Vector3(5, 10, 5);
            Box = TgcBox.fromSize(center, size, texture);

            //Cargo el unico mesh que tiene la escena
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "LogoTGC-TgcScene.xml").Meshes[0];
            //Escalo el mesh que es muy grande
            Mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Se llama en cada frame.
        /// Se debe escribir toda la logica de computo del modelo.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Capturar Input teclado
            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            //Capturar Input Mouse
            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqu� todo el c�digo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);

            //Render de la caja
            Box.render();

            //Render del mesh
            Mesh.render();

            //Render del BoundingBox
            if (BoundingBox)
            {
                Box.BoundingBox.render();
                Mesh.BoundingBox.render();
            }

            //Finaliza el render
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecuci�n del ejemplo.
        ///     Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void Dispose()
        {
            //Dispose de la caja
            Box.dispose();
            //Dispose del mesh
            Mesh.dispose();
        }
    }
}