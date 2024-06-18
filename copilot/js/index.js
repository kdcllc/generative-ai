// write a function that reverses a string
function reverseString(str) {
  return str.split('').reverse().join('');
}

function uppercase(str) {
    // check if str is a string and if it is not return null
    if (typeof str !== 'string') return null;
  return str.toUpperCase();
}

// write a function that sanitizes html input
// and returns it as a string
// that is safe to use in the DOM
// and does not contain any html tags
// or html attributes
// Q: what is the difference between innerText and innerHTML?
// A: innerText will return the text of the element and all its descendants
function sanitizeHTML(str) {
  return str.replace(/<[^>]*>?/gm, '');
}