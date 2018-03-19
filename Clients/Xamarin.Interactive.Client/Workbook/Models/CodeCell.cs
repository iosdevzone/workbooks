//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using CommonMark.Syntax;

using Xamarin.Interactive.CodeAnalysis;

namespace Xamarin.Interactive.Workbook.Models
{
    sealed class CodeCell : Cell, ICellBuffer
    {
        readonly char fenceChar;

        public string LanguageName { get; }
        public string ExtraInfo { get; }
        public override ICellBuffer Buffer => this;

        public event EventHandler BufferChanged;

        string buffer = string.Empty;

        int ICellBuffer.Length => buffer.Length;

        string ICellBuffer.Value {
            get => buffer;
            set {
                buffer = value ?? string.Empty;
                BufferChanged?.Invoke (this, EventArgs.Empty);
            }
        }

        public CodeCell (Block block, string languageName, string extraInfo) : this (
            languageName,
            block.StringContent.ToString (),
            block.FencedCodeData.FenceChar,
            extraInfo)
        {
        }

        public CodeCell (
            string languageName,
            string bufferContents = null,
            char fenceChar = '`',
            string extraInfo = null,
            bool shouldSerialize = true)
        {
            LanguageName = languageName ?? throw new ArgumentNullException (nameof (languageName));
            ExtraInfo = extraInfo;

            if (bufferContents != null)
                Buffer.Value = bufferContents;

            this.fenceChar = fenceChar;
            ShouldSerialize = shouldSerialize;
        }

        string GetFenceInfo ()
            => string.IsNullOrEmpty (ExtraInfo) ? LanguageName : $"{LanguageName} {ExtraInfo}";

        public override Block ToMarkdownDocumentBlock ()
            => ToMarkdownDocumentBlock (
                BlockTag.FencedCode,
                new FencedCodeData {
                    FenceChar = fenceChar,
                    Info = GetFenceInfo ()
                });
    }
}