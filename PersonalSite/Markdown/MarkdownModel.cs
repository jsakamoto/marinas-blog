﻿using Ganss.Xss;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Microsoft.AspNetCore.Components;

namespace PersonalSite.Markdown
{
    public class MarkdownModel : ComponentBase
    {
        private string _content;

        [Inject] public IHtmlSanitizer HtmlSanitizer { get; set; }

        [Parameter]
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                HtmlContent = ConvertStringToMarkupString(_content);
            }
        }

        [Parameter]
        public bool Truncate { get; set; }

        public MarkupString HtmlContent { get; private set; }

        private MarkupString ConvertStringToMarkupString(string value)
        {
            if (!string.IsNullOrWhiteSpace(_content))
            {
                var pipeline = new MarkdownPipelineBuilder()
                        .UseYamlFrontMatter()
                        .UseAdvancedExtensions()
                        .Build();

                //var markdownDocument = Markdig.Markdown.Parse(value, pipeline);

                var html = Markdig.Markdown.ToHtml(value, pipeline); //ToHtml(pipeline, markdownDocument);

                // Sanitize HTML before rendering
                var sanitizedHtml = HtmlSanitizer.Sanitize(html);

                if (Truncate)
                {
                    sanitizedHtml = sanitizedHtml.TruncateHtml(500);
                }

                // Return sanitized HTML as a MarkupString that Blazor can render
                return new MarkupString(sanitizedHtml);
            }

            return new MarkupString();
        }

        private static string ToHtml(MarkdownPipeline pipeline, MarkdownDocument markdownDocument)
        {
            var writer = new StringWriter();

            // Create a HTML Renderer and setup it with the pipeline
            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);

            // Renders markdown to HTML (to the writer)
            renderer.Render(markdownDocument);

            // Gets the rendered string
            var html = writer.ToString();

            return html;
        }
    }
}