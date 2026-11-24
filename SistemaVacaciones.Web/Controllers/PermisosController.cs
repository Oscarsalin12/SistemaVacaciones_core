using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Web.Controllers
{
    public class PermisosController : Controller
    {
        private readonly SistemaVacacionesDbContext _db;
        private readonly IPermisosServices _permisosServices;

        public PermisosController(SistemaVacacionesDbContext db, IPermisosServices permisosServices)
        {
            _db = db;
            _permisosServices = permisosServices;
        }

        private static PermisosPdfVM ToPdfVM(Permiso p) => new()
        {
            Id = p.Id,
            IdEmpleado = p.IdEmpleado,
            IdTipoPermiso = p.IdTipoPermiso,
            IdEmpresa = p.IdEmpresa,
            FechaGeneracion = p.FechaGeneracion,
            FechaInicio = p.FechaInicio,
            FechaFinal = p.FechaFinal,
            DiasDisponibles = p.DiasDisponibles,
            Estado = p.Estado,
            CantidadHorasSolicitadas = p.CantidadHorasSolicitadas,
            HorasDisponibles = p.HorasDisponibles,
            Empleados = p.Empleado,
            SubEmpresas = p.SubEmpresa,
            MotivosAusencia = p.MotivoAusencia
        };

        [HttpGet]
        public IActionResult CrearPermiso(int? idEmpleado)
        {
            var model = new PermisosVM
            {
                Empleados = _permisosServices.ObtenerEmpleados(),
                MotivosAusencia = _permisosServices.ObtenerMotivos(),
                SubEmpresas = _permisosServices.ObtenerEmpresas(),
                FechaGeneracion = DateTime.Today,
                FechaInicio = DateTime.Today,
                FechaFinal = DateTime.Today
            };

            if (idEmpleado.HasValue)
            {
                var empleado = _permisosServices.ObtenerEmpleadoPorId(idEmpleado.Value);
                if (empleado != null)
                {
                    model.IdEmpleado = empleado.Id;
                    model.IdEmpresa = empleado.IdEmpresa;
                    model.NombreEmpleado = empleado.Nombre;
                    model.EmpresaNombre = empleado.SubEmpresa?.Nombre;
                    model.VacacionesDisponibles = empleado.DiasDisponibles;
                    model.HorasDisponibles = empleado.HorasRestantesDelDia;
                }
            }
            ViewBag.EmpresaId = model.IdEmpresa;
            return PartialView("_FormularioPermiso", model);
        }

        [HttpPost]
        public JsonResult CrearPermiso(PermisosVM model)
        {
            if (ModelState.IsValid)
            {
                if (_permisosServices.CrearPermiso(model, out string error))
                {
                    return Json(new { success = true, message = "Permiso registrado Exitosamente" });
                }
                else
                {
                    return Json(new { success = false, message = error });
                }
            }
            return Json(new { success = false, message = "Por favor, verifique los datos ingresados." });
        }

        [HttpGet]
        public IActionResult SolicitarPermiso(int? idEmpleado)
        {
            var model = new PermisosVM
            {
                Empleados = _permisosServices.ObtenerEmpleados(),
                MotivosAusencia = _permisosServices.ObtenerMotivos(),
                SubEmpresas = _permisosServices.ObtenerEmpresas(),
                FechaGeneracion = DateTime.Today
            };

            if (idEmpleado.HasValue)
            {
                var empleado = _permisosServices.ObtenerEmpleadoPorId(idEmpleado.Value);
                if (empleado != null)
                {
                    model.IdEmpleado = empleado.Id;
                    model.IdEmpresa = empleado.IdEmpresa;
                    model.NombreEmpleado = empleado.Nombre;
                    model.EmpresaNombre = empleado.SubEmpresa?.Nombre;
                    model.VacacionesDisponibles = empleado.DiasDisponibles;
                    model.HorasDisponibles = empleado.HorasRestantesDelDia;
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult SolicitarPermiso(PermisosVM model)
        {
            if (ModelState.IsValid)
            {
                if (_permisosServices.CrearSolicitud(model, out string error))
                {
                    TempData["SolicitudEnviada"] = true;
                    return RedirectToAction("PerfilEmpleado", "Empleados");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            }

            model.Empleados = _permisosServices.ObtenerEmpleados();
            model.MotivosAusencia = _permisosServices.ObtenerMotivos();
            model.SubEmpresas = _permisosServices.ObtenerEmpresas();

            return View(model);
        }

        [HttpGet]
        public JsonResult BuscarEmpleado(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Json(null);

            var empleado = _db.Empleados.Include(e => e.SubEmpresa)
                .FirstOrDefault(e => e.Nombre.Contains(filtro) || e.NumEmpleado.ToString() == filtro);

            if (empleado == null)
                return Json(null);

            return Json(new
            {
                id = empleado.Id,
                nombre = empleado.Nombre,
                numEmpleado = empleado.NumEmpleado,
                empresa = empleado.SubEmpresa?.Nombre ?? "",
                empresaId = empleado.IdEmpresa,
                vacaciones = empleado.DiasDisponibles
            });
        }

        public FileResult GenerarPDF(int idEmpresa, string? nombre = null, int? departamento = null, int? motivo = null)
        {
            var permisos = _permisosServices.ObtenerPermisosPorEmpresaConFiltros(idEmpresa, nombre, departamento, motivo);
            var empresa = permisos.FirstOrDefault()?.SubEmpresa?.Empresa;

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.Black);
                var subTituloFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DarkGray);
                var encabezadoFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.White);
                var celdaFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.Black);

                if (empresa?.Logo != null)
                {
                    var logo = iTextSharp.text.Image.GetInstance(empresa.Logo);
                    logo.ScaleAbsolute(70f, 70f);
                    logo.Alignment = Element.ALIGN_LEFT;
                    doc.Add(logo);
                }

                if (empresa != null)
                {
                    doc.Add(new Paragraph(empresa.Nombre, tituloFont));
                    doc.Add(new Paragraph($"RFC: {empresa.Rfc}", subTituloFont));
                    doc.Add(new Paragraph($"Dirección: {empresa.Direccion}", subTituloFont));
                }

                doc.Add(new Paragraph("Fecha de generación: " + DateTime.Now.ToString("dd/MM/yyyy"), subTituloFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20f
                });

                Paragraph titulo = new Paragraph("Reporte de Permisos Aprobados", tituloFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(titulo);

                PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 2.5f, 2.5f, 2.5f, 2.5f, 2f, 1.5f, 1.5f });

                BaseColor headerColor = new BaseColor(0, 122, 204);
                string[] headers = { "Empleado", "Motivo", "Fecha Inicio", "Fecha Final", "Generado", "Días", "Horas" };

                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, encabezadoFont))
                    {
                        BackgroundColor = headerColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                int totalDias = 0;
                int totalHoras = 0;

                foreach (var p in permisos)
                {
                    string nombreEmpleado = p.Empleado?.Nombre ?? "N/A";
                    string motivoAusencia = p.MotivoAusencia?.NombreMotivo ?? "N/A";
                    string fechaInicio = p.FechaInicio.ToString("dd/MM/yyyy");
                    string fechaFinal = p.FechaFinal.ToString("dd/MM/yyyy");
                    string fechaGenerado = p.FechaGeneracion.ToString("dd/MM/yyyy");

                    int diasLaborales = p.Empleado?.DiasLaboralesPorSemana ?? 5;
                    bool descuentaDias = p.MotivoAusencia?.BitDescuentoDias ?? false;
                    bool descuentaHoras = p.MotivoAusencia?.BitDescuentoHoras ?? false;

                    int diasDescontados = (descuentaDias && !descuentaHoras) ? ContarDiasHabiles(p.FechaInicio, p.FechaFinal, diasLaborales) : 0;
                    int horasDescontadas = descuentaHoras ? p.CantidadHorasSolicitadas : 0;

                    totalDias += diasDescontados;
                    totalHoras += horasDescontadas;

                    table.AddCell(new PdfPCell(new Phrase(nombreEmpleado, celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(motivoAusencia, celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(fechaInicio, celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(fechaFinal, celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(fechaGenerado, celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(diasDescontados.ToString(), celdaFont)));
                    table.AddCell(new PdfPCell(new Phrase(horasDescontadas.ToString(), celdaFont)));
                }

                doc.Add(table);
                doc.Add(new Paragraph($"Total Días: {totalDias} | Total Horas: {totalHoras}", subTituloFont) { SpacingBefore = 10f });
                doc.Close();

                Response.Headers["Content-Disposition"] = "inline; filename=Reporte_Permisos.pdf";
                return File(ms.ToArray(), "application/pdf");
            }
        }

        public FileResult GenerarPDFEmpleado(int id)
        {
            var data = _permisosServices.ObtenerPermisosPorEmpleadosA(id);
            var empleado = data.Item1;
            var permisos = data.Item2;
            var empresa = empleado?.SubEmpresa?.Empresa;

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.Black);
                var subTituloFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DarkGray);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.White);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.Black);

                PdfPTable headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                headerTable.SetWidths(new float[] { 2f, 4f });

                if (empresa?.Logo != null)
                {
                    var logo = iTextSharp.text.Image.GetInstance(empresa.Logo);
                    logo.ScaleAbsolute(80f, 80f);
                    PdfPCell logoCell = new PdfPCell(logo)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        PaddingBottom = 10f
                    };
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    headerTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });
                }

                PdfPCell empresaInfo = new PdfPCell
                {
                    Border = Rectangle.NO_BORDER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                empresaInfo.AddElement(new Paragraph(empresa?.Nombre ?? "Empresa", tituloFont));
                empresaInfo.AddElement(new Paragraph("RFC: " + (empresa?.Rfc ?? "N/A"), subTituloFont));
                empresaInfo.AddElement(new Paragraph("Dirección: " + (empresa?.Direccion ?? "N/A"), subTituloFont));
                headerTable.AddCell(empresaInfo);

                doc.Add(headerTable);
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph("Reporte Individual de Permisos", tituloFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                });

                doc.Add(new Paragraph($"Empleado: {empleado?.Nombre ?? "N/A"}", subTituloFont));
                doc.Add(new Paragraph($"Empresa: {empresa?.Nombre ?? "N/A"}", subTituloFont));
                doc.Add(new Paragraph($"Días de vacaciones otorgados: {empleado?.intDiasVacaciones ?? 0}", subTituloFont));
                doc.Add(new Paragraph($"Días disponibles: {empleado?.DiasDisponibles ?? 0}", subTituloFont));
                doc.Add(new Paragraph($"Horas disponibles: {empleado?.HorasRestantesDelDia ?? 0}", subTituloFont));
                doc.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy}", subTituloFont));
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 2f, 2.5f, 2f, 2f, 1.5f, 1.5f });

                BaseColor azulEncabezado = new BaseColor(0, 122, 204);
                string[] headers = { "Fecha Generación", "Motivo", "Inicio", "Final", "Días", "Horas" };

                foreach (var header in headers)
                {
                    table.AddCell(new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = azulEncabezado,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    });
                }

                foreach (var p in permisos)
                {
                    table.AddCell(new PdfPCell(new Phrase(p.FechaGeneracion.ToString("dd/MM/yyyy"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(p.MotivoAusencia?.NombreMotivo ?? "N/A", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(p.FechaInicio.ToString("dd/MM/yyyy"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(p.FechaFinal.ToString("dd/MM/yyyy"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(p.DiasDisponibles.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(p.HorasDisponibles.ToString(), cellFont)));
                }

                doc.Add(table);
                doc.Close();

                Response.Headers["Content-Disposition"] = "inline; filename=Reporte_" + (empleado?.Nombre ?? "Empleado") + ".pdf";
                return File(ms.ToArray(), "application/pdf");
            }
        }

        public FileResult GenerarPDFPermiso(int idPermiso)
        {
            var permiso = _permisosServices.ObtenerPermisoPorId(idPermiso);
            var empleado = permiso?.Empleados;
            var empresaObj = permiso?.SubEmpresas;

            string empresa = empresaObj?.Nombre ?? "N/A";
            string direccion = empresaObj?.Direccion ?? "Dirección no disponible";
            string rfc = empresaObj?.Rfc ?? "RFC no disponible";
            byte[]? logoBytes = empresaObj?.Logo;
            string motivo = permiso?.MotivosAusencia?.NombreMotivo ?? "N/A";

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);

                PdfPCell BorderedCell(Phrase phrase)
                {
                    return new PdfPCell(phrase)
                    {
                        Border = Rectangle.BOX,
                        Padding = 4,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                }

                void AgregarCopia(float yInicio, float altura)
                {
                    PdfContentByte cb = writer.DirectContent;
                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(36, yInicio - altura, PageSize.A4.Width - 36, yInicio);

                    PdfPTable marco = new PdfPTable(1) { WidthPercentage = 100 };
                    marco.DefaultCell.Border = Rectangle.BOX;
                    marco.DefaultCell.Padding = 10;

                    PdfPTable contenido = new PdfPTable(1) { WidthPercentage = 100 };

                    PdfPTable headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                    headerTable.SetWidths(new float[] { 0.3f, 0.7f });

                    if (logoBytes != null)
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoBytes);
                        logo.ScaleToFit(60, 60);
                        headerTable.AddCell(new PdfPCell(logo) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                    }
                    else
                    {
                        headerTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });
                    }

                    PdfPCell empresaCell = new PdfPCell { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                    empresaCell.AddElement(new Paragraph(empresa, boldFont));
                    empresaCell.AddElement(new Paragraph(direccion, normalFont));
                    empresaCell.AddElement(new Paragraph("RFC: " + rfc, normalFont));
                    headerTable.AddCell(empresaCell);

                    contenido.AddCell(new PdfPCell(headerTable) { Border = Rectangle.NO_BORDER, PaddingBottom = 5 });

                    Paragraph titulo = new Paragraph("AVISO DE AUSENCIA", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 10f
                    };
                    contenido.AddCell(new PdfPCell(titulo) { Border = Rectangle.NO_BORDER });

                    PdfPTable datosTabla = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 10, SpacingAfter = 10 };
                    datosTabla.SetWidths(new float[] { 1f, 1f });

                    datosTabla.AddCell(BorderedCell(new Phrase($"No. Folio: {permiso?.Id}", normalFont)));
                    datosTabla.AddCell(BorderedCell(new Phrase($"No. Trab: {empleado?.NumEmpleado}", normalFont)));

                    datosTabla.AddCell(BorderedCell(new Phrase($"Nombre: {empleado?.Nombre}", normalFont)));
                    datosTabla.AddCell(BorderedCell(new Phrase($"Departamento: {empleado?.Departamento?.NombreDep ?? "N/A"}", normalFont)));

                    datosTabla.AddCell(BorderedCell(new Phrase($"Encargado: {empleado?.Departamento?.NombreEmcargado ?? "N/A"}", normalFont)));
                    datosTabla.AddCell(BorderedCell(new Phrase($"Motivo: {motivo}", normalFont)));

                    bool descuentaHoras = permiso?.MotivosAusencia?.BitDescuentoHoras == true;
                    bool descuentaDias = permiso?.MotivosAusencia?.BitDescuentoDias == true;
                    int diasLaborales = (empleado?.DiasLaboralesPorSemana ?? 0) > 0 ? empleado!.DiasLaboralesPorSemana : 5;

                    if (descuentaHoras)
                    {
                        int horasTomadas = permiso?.CantidadHorasSolicitadas ?? 0;
                        int horasRestantes = permiso?.HorasDisponibles ?? 0;
                        datosTabla.AddCell(BorderedCell(new Phrase($"Horas solicitadas: {horasTomadas}", boldFont)));
                        datosTabla.AddCell(BorderedCell(new Phrase($"Restan: {horasRestantes} horas disponibles", boldFont)));
                    }
                    else if (descuentaDias)
                    {
                        int totalDias = ContarDiasHabiles(permiso!.FechaInicio, permiso.FechaFinal, diasLaborales);
                        datosTabla.AddCell(BorderedCell(new Phrase($"Días solicitados: {totalDias}", normalFont)));
                        datosTabla.AddCell(BorderedCell(new Phrase($"Restan: {permiso.DiasDisponibles} días del periodo actual", normalFont)));
                    }
                    else
                    {
                        int totalDias = ContarDiasHabiles(permiso!.FechaInicio, permiso.FechaFinal, diasLaborales);
                        datosTabla.AddCell(BorderedCell(new Phrase($"Días ausente: {totalDias}", normalFont)));
                        datosTabla.AddCell(BorderedCell(new Phrase("Sin descuento de vacaciones", normalFont)));
                    }

                    datosTabla.AddCell(BorderedCell(new Phrase($"Ausente del: {permiso?.FechaInicio:dd-MMMM-yyyy}", normalFont)));
                    datosTabla.AddCell(BorderedCell(new Phrase($"Al: {permiso?.FechaFinal:dd-MMMM-yyyy}", normalFont)));

                    contenido.AddCell(new PdfPCell(datosTabla) { Border = Rectangle.NO_BORDER });

                    PdfPTable firmas = new PdfPTable(3) { WidthPercentage = 100, SpacingBefore = 20 };
                    firmas.AddCell(CreateSignatureCell("RECURSOS HUMANOS"));
                    firmas.AddCell(CreateSignatureCell("ENC. DEPARTAMENTO"));
                    firmas.AddCell(CreateSignatureCell("INTERESADO"));
                    contenido.AddCell(new PdfPCell(firmas) { Border = Rectangle.NO_BORDER, PaddingTop = 10 });

                    Paragraph footer = new Paragraph("Documento Elaborado para RH", normalFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 5f
                    };
                    contenido.AddCell(new PdfPCell(footer) { Border = Rectangle.NO_BORDER });

                    marco.AddCell(contenido);
                    ct.AddElement(marco);
                    ct.Go();
                }

                AgregarCopia(PageSize.A4.Height - 36, (PageSize.A4.Height / 2) - 36);
                AgregarCopia((PageSize.A4.Height / 2) - 36, (PageSize.A4.Height / 2) - 36);

                doc.Close();
                Response.Headers["Content-Disposition"] = "inline; filename=AvisoPermiso_" + (empleado?.Nombre ?? "Permiso") + ".pdf";
                return File(ms.ToArray(), "application/pdf");
            }
        }

        private static PdfPCell CreateSignatureCell(string label)
        {
            return new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 20f,
                Phrase = new Phrase(label, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))
            };
        }

        private static int ContarDiasHabiles(DateTime inicio, DateTime fin, int diasLaboralesPorSemana)
        {
            int diasHabiles = 0;
            DateTime fecha = inicio.Date;

            while (fecha <= fin.Date)
            {
                if (diasLaboralesPorSemana == 5)
                {
                    if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                        diasHabiles++;
                }
                else
                {
                    if (fecha.DayOfWeek != DayOfWeek.Sunday)
                        diasHabiles++;
                }
                fecha = fecha.AddDays(1);
            }
            return diasHabiles;
        }

        [HttpGet]
        public IActionResult PaginacionPorEmpresa(int idEmpresa, int? departamento, int? motivo, string? nombre, int? page, int pageSize = 5)
        {
            int pageNumber = page ?? 1;

            var query = _permisosServices.PermisosPorEmpresa(idEmpresa)
                .Where(p => p.Estado == "Aprobado");

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Empleado != null && p.Empleado.Nombre.Contains(nombre));
            }

            if (departamento.HasValue && departamento.Value > 0)
            {
                query = query.Where(p => p.Empleado != null && p.Empleado.IdDepartamento == departamento.Value);
            }

            if (motivo.HasValue && motivo.Value > 0)
            {
                query = query.Where(p => p.IdTipoPermiso == motivo.Value);
            }

            var resultado = query
                .OrderByDescending(p => p.FechaGeneracion)
                .Select(ToPdfVM)
                .ToList();

            ViewBag.EmpresaId = idEmpresa;
            ViewBag.Departamentos = new SelectList(_db.Departamentos.OrderBy(d => d.NombreDep).ToList(), "Id", "NombreDep", departamento);
            ViewBag.Motivos = new SelectList(_db.MotivosAusencia.OrderBy(m => m.NombreMotivo).ToList(), "Id", "NombreMotivo", motivo);

            return View("Paginacion", resultado.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult CambiarEstado() => View();

        [HttpPost]
        public IActionResult CambiarEstado(int id, string estado)
        {
            var permiso = _permisosServices.ObtenerPermisoPorId(id);

            if (_permisosServices.CambiarEstadoPermiso(id, estado, out string error))
            {
                TempData["EstadoActualizado"] = true;
            }
            else
            {
                TempData["Error"] = error;
            }

            return RedirectToAction("PermisosPendientes", new { idEmpresa = permiso?.IdEmpresa ?? 0 });
        }

        [HttpGet]
        public IActionResult CambiarEstadopag() => View();

        [HttpPost]
        public IActionResult CambiarEstadopag(int id, string estado)
        {
            var permiso = _permisosServices.ObtenerPermisoPorId(id);

            if (_permisosServices.CambiarEstadoPermiso(id, estado, out string error))
            {
                TempData["EstadoActualizado"] = true;
            }
            else
            {
                TempData["Error"] = error;
            }

            return RedirectToAction("PaginacionPorEmpresa", new { idEmpresa = permiso?.IdEmpresa ?? 0 });
        }

        [HttpGet]
        public IActionResult PermisosPendientes(int idEmpresa, int? page, int pageSize = 5)
        {
            int pageNumber = page ?? 1;
            var pendientes = _permisosServices.PermisosPorEmpresa(idEmpresa)
                .Where(p => p.Estado == "Pendiente")
                .OrderBy(p => p.FechaGeneracion)
                .Select(ToPdfVM)
                .ToList();

            ViewBag.EmpresaId = idEmpresa;
            return View("PermisosPendientes", pendientes.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult PermisosEmpleados(int? idEmpresa, int idEmpleado, int? page, int pageSize = 5)
        {
            int pageNumber = page ?? 1;
            var permisos = _permisosServices.ObtenerPermisosConDetalles()
                .Where(p => p.IdEmpleado == idEmpleado)
                .OrderByDescending(p => p.FechaGeneracion)
                .Select(ToPdfVM)
                .ToList();

            ViewBag.EmpleadoId = idEmpleado;
            return View("PermisosEmpleados", permisos.ToPagedList(pageNumber, pageSize));
        }
    }
}
