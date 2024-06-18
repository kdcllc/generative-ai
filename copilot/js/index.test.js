describe('sanitizeHTML', () => {
  it('should remove all HTML tags from a string', () => {
    const input = '<p>Hello, <strong>world!</strong></p>';
    const expectedOutput = 'Hello, world!';
    expect(sanitizeHTML(input)).toEqual(expectedOutput);
  });

  it('should handle empty strings', () => {
    const input = '';
    const expectedOutput = '';
    expect(sanitizeHTML(input)).toEqual(expectedOutput);
  });

  it('should handle strings with no HTML tags', () => {
    const input = 'This is a plain text string.';
    const expectedOutput = 'This is a plain text string.';
    expect(sanitizeHTML(input)).toEqual(expectedOutput);
  });

  it('should handle strings with only HTML tags', () => {
    const input = '<div><span><br></span></div>';
    const expectedOutput = '';
    expect(sanitizeHTML(input)).toEqual(expectedOutput);
  });

  it('should handle strings with mixed HTML and plain text', () => {
    const input = '<p><em>Hello,</em> <strong><a href="#">world!</a></strong></p>';
    const expectedOutput = 'Hello, world!';
    expect(sanitizeHTML(input)).toEqual(expectedOutput);
  });
});