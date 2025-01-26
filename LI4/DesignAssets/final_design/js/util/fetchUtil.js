const fileExists = (file) => fetch(file, {method: "HEAD", cache: "no-store"})
    .then(response => ({200: true, 404: false})[response.status])
    .catch(exception => undefined);

export { fileExists };