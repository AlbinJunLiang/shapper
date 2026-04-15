using System.Net;
using System.Text;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.Orders;

namespace Shapper.Templates
{
    public static class ProductTableTemplate
    {
        public static string GenerateTableHtml(OrderResponseDto order, ExtraDataDto extra)
        {
            var details = order.Details ?? new List<OrderDetailResponseDto>();

            var sb = new StringBuilder();

            sb.Append(
                """
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
    body {
        font-family: Arial, sans-serif;
        margin: 0;
        padding: 0;
    }

    .container {
        width: 100%;
        max-width: 100%;
        padding: 20px;
        box-sizing: border-box;
    }

    .header {
        margin-bottom: 20px;
    }

    .user-box {
        margin-top: 10px;
        padding: 12px;
        border: 1px solid #ddd;
        background: #f9f9f9;
    }

    .totals {
        margin: 20px 0;
    }

    table {
        width: 100%;
        border-collapse: collapse;
        table-layout: fixed;
    }

    th, td {
        border: 1px solid #ddd;
        padding: 8px;
        text-align: left;
        word-break: break-word;
        font-size: 13px;
    }

    th {
        background: #f4f4f4;
    }

    .small {
        font-size: 12px;
        color: #555;
    }

    @media (max-width: 600px) {
        th, td {
            font-size: 11px;
            padding: 6px;
        }
    }
</style>
</head>

<body>
<div class='container'>
"""
            );

            sb.Append(BuildHeader(order, extra));
            sb.Append(BuildTotals(order));
            sb.Append(BuildTable(details));

            sb.Append(
                """
</div>
</body>
</html>
"""
            );

            return sb.ToString();
        }

        private static string BuildHeader(OrderResponseDto order, ExtraDataDto extra)
        {
            return $@"
<div class='header'>
    <h2>Pago confirmado</h2>

    <span>
        Tu pago fue procesado correctamente.
    </span>

    <div class='user-box'>
        <strong>Información del usuario</strong><br/>
        Nombre: {WebUtility.HtmlEncode(extra.Name + " " + extra.LastName)}<br/>
        Email: {WebUtility.HtmlEncode(extra.Email)}<br/>
        Tel: {WebUtility.HtmlEncode(extra.PhoneNumber)}<br/>
        Dirección: {WebUtility.HtmlEncode(extra.Address)}<br/>
        Ciudad: {WebUtility.HtmlEncode(extra.Place)}<br/>
        Código Postal: {WebUtility.HtmlEncode(extra.PostalCode)}
    </div>

    <p><strong>REFERENCIA:</strong> {WebUtility.HtmlEncode(order.OrderReference)}</p>
</div>";
        }

        private static string BuildTotals(OrderResponseDto order)
        {
            return $@"
<div class='totals'>
    <p><strong>Total:</strong> ${order.Total:F2}</p>
    <p><strong>Subtotal:</strong> ${order.Subtotal:F2}</p>
    <p><strong>Impuestos:</strong> ${order.TotalTax:F2}</p>
    <p><strong>Descuento:</strong> ${order.TotalDiscount:F2}</p>
</div>";
        }

        private static string BuildTable(List<OrderDetailResponseDto> details)
        {
            var sb = new StringBuilder();

            sb.Append(
                """
<h3>Detalle de Productos</h3>

<table>
<thead>
<tr>
    <th>Producto</th>
    <th>Descripción</th>
    <th>Cant.</th>
    <th>Recibida</th>
    <th>Base</th>
    <th>Impuesto</th>
    <th>Desc.</th>
    <th>Final</th>
    <th>Subtotal</th>
</tr>
</thead>
<tbody>
"""
            );

            foreach (var item in details)
            {
                sb.Append(
                    $@"
<tr>
    <td>{WebUtility.HtmlEncode(item.ProductName)}</td>
    <td class='small'>{WebUtility.HtmlEncode(item.Description ?? "-")}</td>
    <td>{item.RequestedQuantity}</td>
    <td>{item.ActualQuantity}</td>
    <td>{item.BasePrice:F2}</td>
    <td>{item.Tax:F2}%</td>
    <td>{item.Discount:F2}%</td>
    <td>{item.FinalPrice:F2}</td>
    <td>{item.Subtotal:F2}</td>
</tr>"
                );
            }

            sb.Append(
                """
</tbody>
</table>
"""
            );

            return sb.ToString();
        }
    }
}
