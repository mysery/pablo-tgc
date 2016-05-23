﻿using AlumnoEjemplos.MiGrupo;
using AlumnoEjemplos.PabloTGC.Utiles;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.PabloTGC
{
    /// <summary>
    ///
    /// </summary>
    public class Elemento
    {
        #region Constantes
        public const String Alimento = "Alimento";
        public const String Animal = "Animal";
        public const String Cajon = "Cajon";
        public const String Fuego = "Fuego";
        public const String FuenteAgua = "FuenteAgua";
        public const String Madera = "Madera";
        public const String Olla = "Olla";
        public const String General = "Elemento";
        public const String Copa = "Copa";
        #endregion

        #region Atributos
        private BarraEstado barraEstado;
        #endregion

        #region Propiedades
        public float Peso { get; set; }
        public float Resistencia { get; set; }
        private List<Elemento> ElementosComposicion { get; set; }//Al romperse un obstaculo puede generar otros
        public TgcMesh Mesh { get; set; }
        #endregion

        #region Contructores
        public Elemento()
        {

        }

        public Elemento(TgcMesh mesh, float resistencia)
        {
            this.Mesh = mesh;
            this.Resistencia = resistencia;
            this.barraEstado = new BarraEstado(this.Mesh.BoundingBox.PMin, 
                new Vector3(this.Mesh.BoundingBox.PMin.X, this.Mesh.BoundingBox.PMax.Y, this.Mesh.BoundingBox.PMin.Z), resistencia);

        }

        public Elemento(float peso, float resistencia, TgcBox caja) : this(caja.toMesh("CAJA"), resistencia)//TODO. Pasar el nombre por parámetro
        {
            this.Peso = peso;
            this.ElementosComposicion = new List<Elemento>();
        }

        public Elemento(float peso, float resistencia, TgcMesh mesh) : this(mesh, resistencia)
        {
            this.Peso = peso;
            this.ElementosComposicion = new List<Elemento>();
        }

        public Elemento(float peso, float resistencia, TgcMesh mesh, Elemento elemento) :this(mesh, resistencia)
        {
            this.Peso = peso;
            this.ElementosComposicion = new List<Elemento>();
            this.agregarElemento(elemento);
        }

        #endregion

        #region Comportamientos

        public BarraEstado GetBarraEstado()
        {
            return this.barraEstado;
        }

        /// <summary>
        /// TODO. Ver si no aplica poner una interfaz colisionable
        /// Procesa una colisión cuando la misma es en contra del personaje
        /// </summary>
        public virtual void procesarColision(Personaje personaje, float elapsedTime, List<Elemento> elementos, float moveForward, Vector3 movementVector, Vector3 lastPos)
        {//TODO. Este metodo tiene muchos parametros que deberian ser del personaje.
            if (moveForward < 0)
            {//Si esta caminando para adelante entonces empujamos la caja, sino no hacemos nada.
                if (this.seMueveConUnaFuerza(personaje.fuerza))
                {
                    Vector3 direccionMovimiento = movementVector;
                    direccionMovimiento.Normalize();
                    direccionMovimiento.Multiply(moveForward * elapsedTime * -0.1f);
                    this.mover(direccionMovimiento);
                }
                personaje.mesh.playAnimation("Empujar", true);
                personaje.mesh.Position = lastPos;
                personaje.ActualizarEsferas();
            }
            else
            {
                personaje.mesh.Position = lastPos;
                personaje.ActualizarEsferas();
            }
        }

        public virtual void Actualizar(SuvirvalCraft contexto, float elapsedTime)
        {

        }

        public virtual void procesarInteraccion(String accion, SuvirvalCraft contexto, float elapsedTime)
        {

        }

        public virtual void ProcesarColisionConElemento(Elemento elemento)
        {

        }

        /// <summary>
        /// Aplica el daño que se recibe por parametro y actualiza la barra de estado
        /// </summary>
        /// <returns></returns>
        public void recibirDanio(float danio)
        {
            this.Resistencia -= danio;
            this.barraEstado.ActualizarEstado(this.Resistencia);  
        }

        public bool estaDestruido()
        {
            return this.Resistencia <= 0;
        }

        /// <summary>
        /// Destruye el elemento
        /// </summary>
        public void destruir()
        {
            foreach (Elemento elemento in this.ElementosComposicion)
            {
                elemento.destruir();
            }
            this.Mesh.dispose();
        }
        
        /// <summary>
        /// Destruye el elemento pero deveulve una lista con los elementos que contenia
        /// </summary>
        /// <returns></returns>
        public List<Elemento> DestruirSolo()
        {
            List<Elemento> contenido = new List<Elemento>();
            contenido.AddRange(this.elementosQueContiene());
            this.elementosQueContiene().Clear();
            this.destruir();
            return contenido;
        }

        /// <summary>
        /// Renderiza el objeto
        /// </summary>
        public virtual void renderizar()
        {
            this.Mesh.render();
        }

        public void renderizarBarraEstado()
        {
            this.barraEstado.Render();
        }

        /// <summary>
        /// Caja que encierra al objeto
        /// </summary>
        /// <returns></returns>
        public TgcBoundingBox BoundingBox()
        {
            return this.Mesh.BoundingBox;
        }

        public void mover(Vector3 movimiento)
        {
            this.Mesh.move(movimiento.X, movimiento.Y, movimiento.Z);
        }

        public Vector3 posicion()
        {
            return this.Mesh.Position;
        }

        public void posicion(Vector3 posicion)
        {
            this.Mesh.Position = posicion;
        }

        public void liberar()
        {
            this.Mesh.dispose();
        }

        /// <summary>
        /// TODO. Aplicar de ser posible ecuaciones fisicas de rosamiento y demás de modo tal que sea más real el movimiento.
        /// </summary>
        /// <param name="fuerza"></param>
        public bool seMueveConUnaFuerza(float fuerza)
        {
            return this.Peso < fuerza;
        }

        public void agregarElemento(Elemento elemento)
        {
            this.ElementosComposicion.Add(elemento);
        }

        public void AgregarElementos(List<Elemento> elementos)
        {
            this.ElementosComposicion.AddRange(elementos);
        }

        public void EliminarElemento(Elemento elemento)
        {
            this.ElementosComposicion.Remove(elemento);
        }

        public void EliminarElementos(List<Elemento> elementos)
        {
            foreach (Elemento elem in elementos)
            {
                this.EliminarElemento(elem);
            }
        }

        public List<Elemento> elementosQueContiene()
        {
            return this.ElementosComposicion;
        }

        /// <summary>
        /// Determina si un obstáculo fue destruído completamente o puede arrojar sus partes
        /// </summary>
        /// <returns></returns>
        public bool destruccionTotal()
        {
            if (this.Resistencia <= 0)
            {
                if (this.Resistencia < -1000)
                {
                    return true;
                }
                return false;
            }
            //TODO. Manejar excepciones propias.
            throw new Exception("El obstáculo no esta destruido.");
        }

        public string nombre()
        {
            //TODO. Refactorizar esto
            return this.Mesh.Name;
        }

        public float distanciaA(Elemento unElemento)
        {
            Vector3 aux = new Vector3(unElemento.posicion().X - this.posicion().X,
                unElemento.posicion().Y - this.posicion().Y, unElemento.posicion().Z - this.posicion().Z);
            return aux.Length();
        }

        public float distanciaA(Vector3 posicion)
        {
            Vector3 aux = new Vector3(posicion.X - this.posicion().X,
                posicion.Y - this.posicion().Y, posicion.Z - this.posicion().Z);
            return aux.Length();
        }

        public virtual String getAcciones()
        {
            //El elemento generico no posee acciones
            return "";
        }

        public String GetElementos()
        {
            String elementos = "";
            foreach (Elemento elem in this.ElementosComposicion)
            {
                elementos = elementos + " - " + elem.nombre();
            }
            return elementos;
        }

        public virtual bool AdmiteMultipleColision()
        {
            return false;
        }

        public virtual String GetTipo()
        {
            return General;
        }

        public virtual bool EsDeTipo(String tipo)
        {
            return this.GetTipo().Equals(tipo);
        }

        /// <summary>
        /// Retorne el primer elemento de la lista que cumpla con el tipo que se paso por parametro o null
        /// Deberiamos trabjar con Excepciones
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public Elemento ElementoDeTipo(String tipo)
        {
            Elemento elemento = null;
            foreach (Elemento elem in this.elementosQueContiene())
            {
                if (elem.GetTipo().Equals(tipo))
                {
                    elemento = elem;
                    break;
                }
            }
            return elemento;
        }

        #endregion

    }
}
