# Syncfusion Vue Multiformat PDFViewer

A Vue 2 + ASP.NET Core 10 sample that demonstrates the Syncfusion EJ2 PDF Viewer rendering documents from **17 different file formats** (Word, Excel, PowerPoint, images, and PDF) in a single, unified viewer. Files are uploaded from the browser, converted to PDF on the server using Syncfusion's document renderers, and streamed back to the client for immediate preview, search, annotation, printing, and download — all without leaving the viewer surface.

## Service — File Conversion Pipeline

The backend is an ASP.NET Core 10 Web API (`service/`) that exposes a single endpoint, `POST /PdfViewer/LoadFile`, which accepts a base64-encoded file plus its extension and returns a PDF data URL. The conversion is performed in-memory with the following Syncfusion libraries:

| Input Format | Extensions | Conversion Library |
|---|---|---|
| Word documents | `.doc`, `.docx`, `.dot`, `.dotx`, `.docm`, `.dotm`, `.rtf` | `Syncfusion.DocIORenderer` |
| Excel workbooks | `.xls`, `.xlsx` | `Syncfusion.XlsIORenderer` |
| PowerPoint presentations | `.pptx`, `.pptm`, `.potx`, `.potm` | `Syncfusion.PresentationRenderer` |
| Images | `.jpeg`, `.jpg`, `.png`, `.bmp` | Drawn onto a `PdfPage` using `Syncfusion.Pdf.Graphics` |
| PDF | `.pdf` | Passed through unchanged |

Key characteristics of the service:

- Stateless, in-memory processing — no files are written to disk.
- CORS-enabled (`MyPolicy`) for cross-origin calls from the Vue dev server.
- Returns `data:application/pdf;base64,...` so the client can hand the document straight to the EJ2 PDFViewer.
- 4 MB upload limit (enforced client-side) for non-PDF formats.

## Client — Vue 2 Frontend

The frontend (`vue-app/`) is a Vue CLI 2 project that:

- Provides a drag-and-drop area and a **Browse...** button powered by the Syncfusion Uploader component.
- Validates the file type against the 17 supported extensions and enforces the 4 MB size cap for non-PDF inputs.
- Shows a linear progress bar that combines upload progress and PDFViewer load progress.
- Renders the converted PDF in the Syncfusion EJ2 PDFViewer with toolbar, navigation, magnification, search, annotations, form fields, form designer, print, and download enabled.
- Uses the **Material 3** theme via `@syncfusion/ej2-material3-theme`.

## Architecture

- **service/** – ASP.NET Core 10 Web API (`PdfViewerController.cs`, `Program.cs`)
- **vue-app/** – Vue 2 + Vue CLI + Syncfusion EJ2 Vue components (`App.vue`, `main.js`)

## Getting Started

### Server

```bash
cd service
dotnet run
```

### Client
```bash
cd vue-app
npm install
npm run serve
```