﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ProyectoFinal_DBD.Helpers;
using ProyectoFinal_DBD.Models;
using Oracle.DataAccess.Client;
using System.Data;

namespace ProyectoFinal_DBD.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Objeto de conexión a base de datos Oracle
        /// </summary>
        private OracleConn oracleCon;

        private ModelMapper mapper = new ModelMapper();

        /// <summary>
        /// Muestra el listado de cuentas del sistema
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Obtiene el listado de cuentas del sistema
            oracleCon = new OracleConn();
            DataTable dtCuentas = oracleCon.ExecuteQuery("SELECT * FROM CUENTA");
            List<CuentaModel> listaCuentas = mapper.MapearModeloCuenta(dtCuentas);

            ViewBag.ActiveHome = "active";
            return View(listaCuentas);
        }

        /// <summary>
        /// Muestra la vista para guardar una nueva cuenta
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NuevaCuenta()
        {
            ViewBag.ActiveNew = "active";
            return View(new CuentaModel());
        }

        /// <summary>
        /// Guarda la nueva cuenta ingresada y devuelve la vista principal
        /// </summary>
        /// <param name="cuentaModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NuevaCuenta(CuentaModel cuentaModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveNew = "active";
                return View(cuentaModel);
            }

            Guid value = Guid.NewGuid();
            cuentaModel.Cuenta = value.ToString().Substring(0, 11).Replace("-", "");

            oracleCon = new OracleConn();

            List<OracleParameter> listaParametros = new List<OracleParameter>();

            OracleParameter parametro = new OracleParameter();

            parametro = new OracleParameter("vl_cuenta", OracleDbType.NVarchar2, cuentaModel.Cuenta, ParameterDirection.Input);
            listaParametros.Add(parametro);
            parametro = new OracleParameter("vl_nombre", OracleDbType.NVarchar2, cuentaModel.Nombre, ParameterDirection.Input);
            listaParametros.Add(parametro);
            parametro = new OracleParameter("vl_apellido", OracleDbType.NVarchar2, cuentaModel.Apellido, ParameterDirection.Input);
            listaParametros.Add(parametro);
            parametro = new OracleParameter("vl_saldo", OracleDbType.Decimal, cuentaModel.Saldo, ParameterDirection.Input);
            listaParametros.Add(parametro);
            parametro = new OracleParameter("vl_interes", OracleDbType.Decimal, cuentaModel.Interes, ParameterDirection.Input);
            listaParametros.Add(parametro);

            String resultado = oracleCon.ExecuteProcedure("pkg_account_management.insert_account", listaParametros);

            ViewBag.ActiveNew = "active";
            return View("Index");
        }
    }
}