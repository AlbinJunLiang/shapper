// Helpers/ValidationHelper.cs
using Shapper.Dtos.OrderDetails;

namespace Shapper.Templates
{
    public static class ProductTableTemplate
    {
        public static string GenerateTableHtml(List<OrderDetailResponseDto> details)
        {
            var html =
                @"
    <html>
    <head>
        <style>
            body {
                font-family: Arial, sans-serif;
            }
            table {
                width: 100%;
                border-collapse: collapse;
            }
            th, td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
                word-break: break-word;
                max-width: 140px;
            }
            th {
                background-color: #f4f4f4;
            }
            .small {
                font-size: 12px;
                color: #555;
            }
        </style>
    </head>
    <body>

        <h3>Detalle de Productos</h3>

        <table>
            <thead>
                <tr>
                    <th>Producto</th>
                    <th>Descripción</th>
                    <th>Cant. Solicitada</th>
                    <th>Cant. Recibida</th>
                    <th>Precio Base</th>
                    <th>Impuesto</th>
                    <th>Descuento</th>
                    <th>Precio Final</th>
                    <th>Subtotal</th>
                </tr>
            </thead>
            <tbody>";

            foreach (var item in details)
            {
                var description = string.IsNullOrWhiteSpace(item.Description)
                    ? "-"
                    : item.Description;

                html +=
                    $@"
                <tr>
                    <td>{item.ProductName ?? "-"}</td>
                    <td class='small'>{description}</td>
                    <td>{item.RequestedQuantity}</td>
                    <td>{item.ActualQuantity}</td>
                    <td>{item.BasePrice:F2}</td>
                    <td>{item.Tax:F2}%</td>
                    <td>{item.Discount:F2}%</td>
                    <td>{item.FinalPrice:F2}</td>
                    <td>{item.Subtotal:F2}</td>
                </tr>";
            }

            var total = details.Sum(x => x.Subtotal);

            html +=
                $@"
            </tbody>
        </table>

        <p><strong>Total: {total:F2}</strong></p>

    </body>
    </html>";

            return html;
        }
    }
}
