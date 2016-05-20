﻿using AlumnoEjemplos.PabloTGC.Instrumentos;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.PabloTGC
{
    public class Personaje
    {
        #region Atributos
        private TgcSphere boundingEsfera;
        private TgcSphere alcanceInteraccionEsfera;
        #endregion

        #region Propiedades
        public float fuerza { get; set; }
        public float golpe { get; set; }//TODO este atributo esta al pedo, creo
        public TgcSkeletalMesh mesh { get; set; }
        public float velocidadCaminar { get; set; }
        public float velocidadRotacion { get; set; }
        public float salud { get; set; }
        public float tiempoCorriendo { get; set; }
        public float resistenciaFisica { get; set; }
        private List<Elemento> mochila { get; set; }

        private Arma instrumentoManoDerecha { get; set; }//TODO. queda pendiente que las armas extiendan de algun objeto en comun.
        private List<Arma> instrumentos { get; set; }//TODO. queda pendiente que las armas extiendan de algun objeto en comun.
        #endregion

        #region Constructores

        public Personaje()
        {
            this.mochila = new List<Elemento>();
            this.instrumentos = new List<Arma>();
            this.tiempoCorriendo = 0;
        }
        #endregion

        #region Comportamientos

        public void IniciarBoundingEsfera()
        {
            //Esta esfera seria el "bounding box" del personaje
            boundingEsfera = new TgcSphere();
            boundingEsfera.setColor(Color.SkyBlue);
            boundingEsfera.Radius = 60;
            boundingEsfera.Position = this.mesh.Position;
            boundingEsfera.updateValues();
        }

        public void IniciarAlcanceInteraccionEsfera()
        {
            alcanceInteraccionEsfera = new TgcSphere();
            alcanceInteraccionEsfera.setColor(Color.White);
            alcanceInteraccionEsfera.Radius = 70;
            alcanceInteraccionEsfera.Position = this.mesh.Position;
            alcanceInteraccionEsfera.updateValues();
        }

        public void ActualizarEsferas()
        {
            //Actualizamos la esfera del bounding
            boundingEsfera.Position = this.mesh.Position + new Vector3(0, (this.mesh.BoundingBox.PMax.Y - this.mesh.BoundingBox.PMin.Y) / 2, 0);
            boundingEsfera.updateValues();

            //Actualizamos la esfera del alcance
            //TODO. Misma logica de siempre para saber la direccion del persnaje. REVISAR.
            Vector3 direccionEsferaGolpe = this.mesh.Position + new Vector3(-(float)Math.Sin((float)this.mesh.Rotation.Y) * 50,
                (this.mesh.BoundingBox.PMax.Y - this.mesh.BoundingBox.PMin.Y) / 2, -(float)Math.Cos((float)this.mesh.Rotation.Y) * 50);
            alcanceInteraccionEsfera.Position = direccionEsferaGolpe;
            alcanceInteraccionEsfera.updateValues();
        }

        public TgcSphere GetBoundingEsfera()
        {
            return this.boundingEsfera;
        }

        public TgcSphere GetAlcanceInteraccionEsfera()
        {
            return this.alcanceInteraccionEsfera;
        }

        public void agregarInstrumento(Arma instrumento)
        {
            this.instrumentos.Add(instrumento);
        }

        public void juntar(Elemento elemento)
        {
            this.mochila.Add(elemento);
        }

        public void Dejar(Elemento elemento)
        {
            this.mochila.Remove(elemento);
        }

        public void DejarElementos(List<Elemento> elementos)
        {
            foreach (Elemento elem in elementos)
            {
                this.Dejar(elem);
            }
        }

        public float correr(float tiempo)
        {
            if (this.tiempoCorriendo >= this.resistenciaFisica)
            {
                this.tiempoCorriendo = this.resistenciaFisica;
                return this.velocidadCaminar;
            }
            else
            {
                this.tiempoCorriendo += tiempo;
                if (this.tiempoCorriendo >= this.resistenciaFisica * 0.5)
                {
                    return this.velocidadCaminar * 1.5f;
                }
                return this.velocidadCaminar * 3.5f;
            }
        }

        public float rotarRapido()
        {
            return this.velocidadRotacion * 2.5f;
        }

        /// <summary>
        /// Afecta la salud del personaje dado un tiempo pasado por parámetro. El tiempo es en segundos.
        /// </summary>
        /// <param name="tiempoEnSegundos"></param>
        public void afectarSaludPorTiempo(float tiempoEnSegundos)
        {
            //Afectamos la salud de esta manera tan agresiva para que el juego sea más interactivo
            this.salud -= tiempoEnSegundos;
        }

        public void incrementoResistenciaFisica(float tiempoEnSegundos)
        {
            if ((this.tiempoCorriendo - tiempoEnSegundos) > 0)
            {
                this.tiempoCorriendo -= tiempoEnSegundos;
            }
            else
            {
                this.tiempoCorriendo = 0;
            }
        }

        public bool estaMuerto()
        {
            return this.salud <= 0;
        }

        public void consumirAlimento()
        {
            //TODO. Este método debe ser definido en forma mas precisa
            this.salud = 100;
        }

        public void incrementarSaludPorTiempo(float tiempoEnSegundos)
        {
            //TODO. Este método debe ser definido en forma mas precisa
            this.salud += tiempoEnSegundos * 1.2f;
            if (this.salud > 100)
            {
                this.salud = 100;
            }
        }

        public void morir()
        {
            this.salud = 0;
        }

        public List<Elemento> elementosEnMochila()
        {
            return this.mochila;
        }

        /// <summary>
        /// El orden en el cual fueron cargados los instrumentos es el orden que se utilizará para seleccionar el instrumento
        /// </summary>
        /// <param name="numeroInstrumento"></param>
        public void seleccionarInstrumentoManoDerecha(int numeroInstrumento)
        {
            //Por el momento sabemos que el Attachment 0 es el que esta en la mano derecha
            this.instrumentoManoDerecha = this.instrumentos[numeroInstrumento];
            this.mesh.Attachments[0].Mesh = this.instrumentoManoDerecha.mesh;
            this.mesh.Attachments[0].Offset = this.instrumentoManoDerecha.translacion;
            this.mesh.Attachments[0].updateValues();
        }

        public float alcancePatada()
        {
            return 60;
        }

        public float fuerzaPatada()
        {
            return 66;
        }

        public float alcanceGolpe()
        {
            return this.instrumentoManoDerecha.alcance;
        }

        public float fuerzaGolpe()
        {
            return this.instrumentoManoDerecha.potenciaGolpe * this.fuerza;
        }

        #endregion
    }
}