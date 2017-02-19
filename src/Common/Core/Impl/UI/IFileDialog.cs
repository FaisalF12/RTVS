// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.Common.Core.UI {
    public interface IFileDialog {
        /// <summary>
        /// Shows the open file dialog.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="initialPath"></param>
        /// <param name="title"></param>
        /// <returns>Full path to the file selected, or <c>null</c>.</returns>
        string ShowOpenFileDialog(string filter, string initialPath = null, string title = null);

        /// <summary>
        /// Shows the browse directory dialog.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="initialPath"></param>
        /// <param name="title"></param>
        /// <returns>Full path to the directory selected, or <c>null</c>.</returns>
        string ShowBrowseDirectoryDialog(string initialPath = null, string title = null);

        /// <summary>
        /// Shows the save file dialog.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="initialPath"></param>
        /// <param name="title"></param>
        /// <returns>Full path to the file selected, or <c>null</c>.</returns>
        string ShowSaveFileDialog(string filter, string initialPath = null, string title = null);

        /// <summary>
        /// Show the export image dialog box.
        /// </summary>
        /// <param name="imageArguments"></param>
        /// <param name="filter"></param>
        /// <param name="initialPath"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        ExportImageParameters ShowExportImageDialog(ExportArguments imageArguments, string filter, string initialPath = null, string title = null);

        /// <summary>
        /// Show the export PDF dialog box.
        /// </summary>
        /// <param name="pdfArguments"></param>
        /// <param name="filter"></param>
        /// <param name="initialPath"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        ExportPdfParameters ShowExportPdfDialog(ExportArguments pdfArguments,string filter, string initialPath = null, string title = null);
    }
}