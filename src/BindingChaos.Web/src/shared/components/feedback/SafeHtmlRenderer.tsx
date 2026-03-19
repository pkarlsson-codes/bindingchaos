import React from 'react';
import DOMPurify from 'dompurify';

interface SafeHtmlRendererProps {
  html: string;
  className?: string;
}

export function SafeHtmlRenderer({ html, className = "" }: SafeHtmlRendererProps) {
  // Use DOMPurify for robust HTML sanitization
  const sanitizedHtml = DOMPurify.sanitize(html, {
    // Allow common HTML elements that TipTap generates
    ALLOWED_TAGS: [
      'p', 'br', 'strong', 'b', 'em', 'i', 'u', 's', 'strike', 'del',
      'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
      'ul', 'ol', 'li',
      'blockquote', 'code', 'pre',
      'table', 'thead', 'tbody', 'tr', 'td', 'th',
      'a', 'img'
    ],
    // Allow common attributes
    ALLOWED_ATTR: [
      'href', 'src', 'alt', 'title', 'target', 'rel',
      'class', 'style', 'width', 'height'
    ],
    // Allow data attributes for TipTap
    ALLOW_DATA_ATTR: true,
    // Allow relative URLs
    ALLOWED_URI_REGEXP: /^(?:(?:(?:f|ht)tps?|mailto|tel|callto|cid|xmpp):|[^a-z]|[a-z+.\-]+(?:[^a-z+.\-:]|$))/i
  });

  return (
    <div 
      className={className}
      dangerouslySetInnerHTML={{ __html: sanitizedHtml }}
    />
  );
} 