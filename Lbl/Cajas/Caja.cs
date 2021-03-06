using Lazaro.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.Text;


namespace Lbl.Cajas
{
        [Lbl.Atributos.Nomenclatura(NombreSingular = "Caja", Grupo = "Cajas")]
        [Lbl.Atributos.Datos(TablaDatos = "cajas", CampoId = "id_caja")]
        [Lbl.Atributos.Presentacion()]
	public class Caja : ElementoDeDatos, Lbl.ICamposBaseEstandar, Lbl.ICuenta, Lbl.IEstadosEstandar
	{
                Lazaro.Base.Controller.CajaController CajaController = null;

                private Lbl.Bancos.Banco m_Banco = null;
                private Lbl.Entidades.Moneda m_Moneda = null;

		//Heredar constructores
                public Caja(Lfx.Data.IConnection dataBase) 
                        : base(dataBase)
                {
                        CajaController = new Lazaro.Base.Controller.CajaController(
                                new Lazaro.Orm.EntityManager(this.Connection, Lfx.Workspace.Master.MetadataFactory),
                                this
                                );
                }

		public Caja(Lfx.Data.IConnection dataBase, int itemId)
			: base(dataBase, itemId)
                {
                        CajaController = new Lazaro.Base.Controller.CajaController(
                                new Lazaro.Orm.EntityManager(this.Connection, Lfx.Workspace.Master.MetadataFactory),
                                this
                                );
                }

                public Caja(Lfx.Data.IConnection dataBase, Lfx.Data.Row row)
                        : base(dataBase, row)
                {
                        CajaController = new Lazaro.Base.Controller.CajaController(
                                new Lazaro.Orm.EntityManager(this.Connection, Lfx.Workspace.Master.MetadataFactory),
                                this
                                );
                }


                /// <summary>
                /// Obtiene o establece el nombre del elemento.
                /// </summary>
                [Column(Name = "nombre")]
                public virtual string Nombre
                {
                        get
                        {
                                return this.GetFieldValue<string>(CampoNombre);
                        }
                        set
                        {
                                this.Registro[CampoNombre] = value;
                        }
                }


                /// <summary>
		/// Obtiene o establece un texto que representa las observaciones del elemento.
		/// </summary>
		[Column(Name = "obs")]
                public string Obs
                {
                        get
                        {
                                if (this.Registro["obs"] == null || this.Registro["obs"] == DBNull.Value)
                                        return null;
                                else
                                        return this.Registro["obs"].ToString();
                        }
                        set
                        {
                                if (value == null) {
                                        this.Registro["obs"] = null;
                                } else {
                                        this.Registro["obs"] = value.Trim(new char[] { '\n', '\r', ' ' });
                                }
                        }
                }


                [Column(Name = "fecha")]
                public DateTime Fecha
                {
                        get
                        {
                                return this.GetFieldValue<DateTime>("fecha");
                        }
                }


                /// <summary>
                /// Devuelve o establece el estado del elemento. El valor de esta propiedad tiene diferentes significados para cada clase derivada.
                /// </summary>
                [Column(Name = "estado")]
                public int Estado
                {
                        get
                        {
                                return this.GetFieldValue<int>("estado");
                        }
                        set
                        {
                                this.Registro["estado"] = value;
                        }
                }


                /// <summary>
                /// Obtiene el saldo actual de la caja.
                /// </summary>
                /// <param name="forUpdate">Indica si es una consulta con fines de actualizar el saldo.</param>
                /// <returns>El saldo en la moneda de la caja.</returns>
                public virtual decimal ObtenerSaldo(bool forUpdate)
		{
                        return this.CajaController.ObtenerSaldo(forUpdate);
		}


                /// <summary>
                /// Obtiene el �ltimo movimiento de la caja.
                /// </summary>
                /// <returns>El �ltimo movimiento registrado en la caja.</returns>
                public Movimiento ObtenerUltimoMovimiento()
                {
                        return this.CajaController.ObtenerUltimoMovimiento();
                }


                public Bancos.Banco Banco
                {
                        get
                        {
                                if (m_Banco == null && this.GetFieldValue<int>("id_banco") > 0)
                                        m_Banco = new Bancos.Banco(this.Connection, this.GetFieldValue<int>("id_banco"));
                                return m_Banco;
                        }
                        set
                        {
                                m_Banco = value;
                                this.SetFieldValue("id_banco", value);
                        }
                }


                public Entidades.Moneda ObtenerMoneda()
                {
                        if (this.Moneda == null)
                                return Lbl.Sys.Config.Moneda.MonedaPredeterminada;
                        else
                                return this.Moneda;
                }


                public Entidades.Moneda Moneda
                {
                        get
                        {
                                if (m_Moneda == null && this.GetFieldValue<int>("id_moneda") > 0)
                                        m_Moneda = new Entidades.Moneda(this.Connection, this.GetFieldValue<int>("id_moneda"));
                                return m_Moneda;
                        }
                        set
                        {
                                m_Moneda = value;
                                this.SetFieldValue("id_moneda", value);
                        }
                }

                public string Titular
                {
                        get
                        {
                                return this.GetFieldValue<string>("titular");
                        }
                        set
                        {
                                this.Registro["titular"] = value;
                        }
                }

                public string Numero
                {
                        get
                        {
                                return this.GetFieldValue<string>("numero");
                        }
                        set
                        {
                                this.Registro["numero"] = value;
                        }
                }

                public string ClaveBancaria
                {
                        get
                        {
                                return this.GetFieldValue<string>("cbu");
                        }
                        set
                        {
                                this.Registro["cbu"] = value;
                        }
                }

                public TiposDeCaja Tipo
                {
                        get
                        {
                                return (TiposDeCaja)(this.GetFieldValue<int>("tipo"));
                        }
                        set
                        {
                                this.Registro["tipo"] = (int)value;
                        }
                }

                public override Lfx.Types.OperationResult Guardar()
                {
                        qGen.IStatement Comando;
                        if (this.Existe == false) {
                                Comando = new qGen.Insert(this.TablaDatos);
                                Comando.ColumnValues.AddWithValue("fecha", new qGen.SqlExpression("NOW()"));
                        } else {
                                Comando = new qGen.Update(this.TablaDatos);
                                Comando.WhereClause = new qGen.Where(this.CampoId, this.Id);
                        }

                        Comando.ColumnValues.AddWithValue(this.CampoNombre, this.Nombre);
                        Comando.ColumnValues.AddWithValue("titular", this.Titular);
                        if (Banco == null)
                                Comando.ColumnValues.AddWithValue("id_banco", null);
                        else
                                Comando.ColumnValues.AddWithValue("id_banco", this.Banco.Id);
                        if (this.Moneda == null)
                                Comando.ColumnValues.AddWithValue("id_moneda", null);
                        else
                                Comando.ColumnValues.AddWithValue("id_moneda", this.Moneda.Id);
                        Comando.ColumnValues.AddWithValue("numero", this.Numero);
                        Comando.ColumnValues.AddWithValue("tipo", (int)(this.Tipo));
                        Comando.ColumnValues.AddWithValue("cbu", this.ClaveBancaria);
                        Comando.ColumnValues.AddWithValue("estado", this.Estado);

                        Connection.ExecuteNonQuery(Comando);

                        return base.Guardar();
                }

                public void Arqueo()
                {
                        this.Movimiento(false, null, "Arqueo de caja - Saldo: " + Lbl.Sys.Config.Moneda.Simbolo + " " + Lfx.Types.Formatting.FormatCurrency(this.ObtenerSaldo(false)), Lbl.Sys.Config.Actual.UsuarioConectado.Persona, 0, null, null, null, null);
                }

                public void Movimiento(bool auto, Lbl.Cajas.Concepto concepto, string textoConcepto,
                        Lbl.Personas.Persona cliente, decimal importe, string obs,
                        Lbl.Comprobantes.ComprobanteConArticulos factura, Lbl.Comprobantes.Recibo recibo, string comprobantes)
                {
                       CajaController = new Lazaro.Base.Controller.CajaController(
                                new Lazaro.Orm.EntityManager(this.Connection, Lfx.Workspace.Master.MetadataFactory),
                                this
                                );
                        CajaController.Movimiento(auto, concepto, textoConcepto,
                        cliente, importe, obs,
                        factura, recibo, comprobantes);
                     
                }


                /// <summary>
                /// Recalcula completamente el saldo de la caja, para corregir errores de transporte. Principalmente de uso interno.
                /// </summary>
                public void Recalcular()
                {
                        CajaController.Recalcular();
                }


                public void Activar(bool activar)
                {
                        this.Estado = 0;
                        qGen.Update ActCmd = new qGen.Update(this.TablaDatos);
                        ActCmd.ColumnValues.AddWithValue("estado", activar ? 1 : 0);
                        ActCmd.WhereClause = new qGen.Where(this.CampoId, this.Id);
                        this.Connection.ExecuteNonQuery(ActCmd);
                        Lbl.Sys.Config.ActionLog(this.Connection, Lbl.Sys.Log.Acciones.Delete, this, activar ? "Activar" : "Desactivar");
                }
	}
}
