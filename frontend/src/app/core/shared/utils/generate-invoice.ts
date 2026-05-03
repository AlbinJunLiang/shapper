import { OrderResponse } from "../../interfaces/order-data.interface";
import { environment } from "../../../../environments/environment.development";

const generatePDF = async (
  order: OrderResponse,
  storeLink: string
) => {

  const pdfMakeModule = await import("pdfmake/build/pdfmake");
  const pdfFontsModule = await import("pdfmake/build/vfs_fonts");

  const pdfMake = pdfMakeModule.default;
  const pdfFonts = pdfFontsModule;

  (pdfMake as any).vfs = pdfFonts.pdfMake.vfs;
  (pdfMake as any).vfs = pdfFonts.pdfMake.vfs;


  const orderDetails = order.details;
  const currency = environment.currency;

  const tableBody = [
    [
      { text: "Product", style: "tableHeader" },
      { text: "Description", style: "tableHeader" },
      { text: "Qty Requested", style: "tableHeader", alignment: 'center' },
      { text: "Qty Granted", style: "tableHeader", alignment: 'center' },
      { text: "Unit Price", style: "tableHeader", alignment: 'right' },
      { text: "Discount", style: "tableHeader", alignment: 'right' },
      { text: "Tax", style: "tableHeader", alignment: 'right' },
      { text: "Total", style: "tableHeader", alignment: 'right' },
    ],
    ...orderDetails.map((item) => [
      { text: item.productName || 'N/A', style: "tableCell" },
      { text: item.description || 'N/A', style: "tableCell" },
      { text: (item.requestedQuantity || 0).toString(), alignment: 'center' },
      { text: (item.actualQuantity || 0).toString(), alignment: 'center' },
      { text: `${currency}${(item.basePrice || 0).toFixed(2)}`, alignment: 'right' },
      { text: `%${(item.discount || 0)}`, alignment: 'right' },
      { text: `%${(item.tax || 0)}`, alignment: 'right' },
      { text: `${currency}${(item.subtotal || 0).toFixed(2)}`, alignment: 'right', bold: true },
    ]),
  ];

  const content: any[] = [];

  content.push({
    columns: [
      {
        stack: [
          { text: `Order Reference: ${order.orderReference}`, style: "header", margin: [0, 0, 0, 10] },
          { text: `Customer: ${order.extraData.name || ""} ${order.extraData.lastName || ""}`.trim(), style: "subheader" },
          { text: `Address: ${order.extraData.address || "N/A"}`, style: "subheader" },
          { text: `Phone: ${order.extraData.phoneNumber || "N/A"}`, style: "subheader" },
          { text: `Email: ${order.extraData.email || "N/A"}`, style: "subheader" },
          {
            text: `Date: ${new Date(order.createdAt).toLocaleString('en-US', {
              day: '2-digit',
              month: 'long',
              year: 'numeric',
              hour: '2-digit',
              minute: '2-digit'
            })}`,
            style: "subheader",
            margin: [0, 10, 0, 0]
          }
        ],
        alignment: "right",
      },
    ],
    margin: [0, 0, 0, 20]
  });

  content.push({
    qr: storeLink ?? 'http://localhost:4200/orders',
    fit: 100,
    alignment: "right",
    margin: [0, 10, 0, 10],
  });

  content.push({ text: "\n" });

  content.push({
    table: {
      headerRows: 1,
      widths: ["*", "auto", "auto", "auto", "auto", "auto", "auto", "auto"],
      body: tableBody,
    },
    layout: "lightHorizontalLines",
    margin: [0, 10, 0, 10],
  });

  content.push({
    columns: [
      { text: "", width: "*" },
      {
        width: "auto",
        stack: [
          {
            columns: [
              { text: "Shipping", width: 80, color: '#666666', fontSize: 10 },
              { text: `${currency}${(order.shippingCost || 0).toFixed(2)}`, width: 80, alignment: 'right' }
            ],
            margin: [0, 2, 0, 2]
          },
          {
            columns: [
              { text: "Subtotal", width: 80, color: '#666666' },
              { text: `${currency}${(order.subtotal || 0).toFixed(2)}`, width: 80, alignment: 'right' }
            ],
            margin: [0, 2, 0, 2]
          },
          {
            columns: [
              { text: "Discount", width: 80, color: '#10b981' },
              { text: `- ${currency}${(order.totalDiscount || 0).toFixed(2)}`, width: 80, alignment: 'right', color: '#10b981' }
            ],
            margin: [0, 2, 0, 2]
          },
          {
            columns: [
              { text: "Tax (IVA)", width: 80, color: '#666666' },
              { text: `${currency}${(order.totalTax || 0).toFixed(2)}`, width: 80, alignment: 'right' }
            ],
            margin: [0, 2, 0, 2]
          },
          {
            canvas: [{ type: 'line', x1: 0, y1: 5, x2: 160, y2: 5, lineWidth: 1, lineColor: '#eeeeee' }],
            margin: [0, 5, 0, 5]
          },
          {
            columns: [
              { text: "Grand Total", width: 80, bold: true, fontSize: 13 },
              { text: `${currency}${(order.total || 0).toFixed(2)}`, width: 80, alignment: 'right', bold: true, fontSize: 13 }
            ],
            margin: [0, 5, 0, 0]
          }
        ]
      }
    ]
  });

  const styles = {
    header: { fontSize: 14, bold: true },
    subheader: { fontSize: 12, margin: [0, 5, 0, 5] },
    tableHeader: { bold: true, fontSize: 12, color: "black" },
    total: { fontSize: 12, bold: true },
  };

  const docDefinition: any = {
    content,
    styles,
  };


  pdfMake.createPdf(docDefinition).download(
    `Order-${order.orderReference}.pdf`
  );
};

export default generatePDF;