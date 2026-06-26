using ISLAGO_V3.Entidad.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ISLAGO_V3.Helpers
{
    public class CompraPdf : IDocument
    {
        private readonly Compra _compra;
        private readonly List<DetalleCompra> _detalles;

        public CompraPdf(
            Compra compra,
            List<DetalleCompra> detalles)
        {
            _compra = compra;
            _detalles = detalles;
        }

        public DocumentMetadata GetMetadata()
            => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                // ==========================
                // ENCABEZADO
                // ==========================

                page.Header()
                    .Column(col =>
                    {
                        col.Item()
                            .Text("FERRETERÍA LAGO")
                            .FontSize(22)
                            .Bold();

                        col.Item()
                            .Text($"Compra #{_compra.Id}");

                        col.Item()
                            .Text($"Fecha: {_compra.Fecha:dd/MM/yyyy}");

                        col.Item()
                            .Text($"Proveedor: {_compra.IdproveedorNavigation?.IdpersonaNavigation?.Nombres}");
                    });

                // ==========================
                // CONTENIDO
                // ==========================

                page.Content()
                    .PaddingVertical(20)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Artículo").Bold();
                            header.Cell().Text("Cantidad").Bold();
                            header.Cell().Text("Precio").Bold();
                            header.Cell().Text("Subtotal").Bold();
                        });

                        foreach (var d in _detalles)
                        {
                            table.Cell().Text(d.IdarticuloNavigation?.Nombre);

                            table.Cell().Text(
                                d.Cantidad?.ToString());

                            table.Cell().Text(
                                $"C$ {d.Precio}");

                            table.Cell().Text(
                                $"C$ {d.Subtotal}");
                        }
                    });

                // ==========================
                // PIE
                // ==========================

                page.Footer()
                    .AlignRight()
                    .Text($"TOTAL: C$ {_compra.Total}");
            });
        }
    }
}