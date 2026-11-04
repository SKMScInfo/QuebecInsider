import { AngularNodeAppEngine, createNodeRequestHandler, isMainModule, writeResponseToNodeResponse } from '@angular/ssr/node';
import express from 'express';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const serverDistFolder = dirname(fileURLToPath(import.meta.url));
const browserDistFolder = resolve(serverDistFolder, '../browser');
const app = express();
const angularApp = new AngularNodeAppEngine();
const apiBaseUrl = (
  process.env['API_URL'] ||
  'https://ca-quebecinsider-api.delightfulbeach-019eb41d.westus.azurecontainerapps.io'
).replace(/\/+$/, '');

app.use('/api', async (req, res, next) => {
  if (req.path === '/ssr' || req.originalUrl.startsWith('/api/ssr')) {
    return next();
  }
  try {
    const targetUrl = `${apiBaseUrl}${req.originalUrl}`;
    const response = await fetch(targetUrl, {
      method: req.method,
      headers: { Accept: 'application/json' }
    });
    res.status(response.status);
    const contentType = response.headers.get('content-type');
    if (contentType) {
      res.setHeader('Content-Type', contentType);
    }
    const body = await response.text();
    res.send(body);
  } catch (error) {
    next(error);
  }
});

app.use(express.static(browserDistFolder, { maxAge: '1y', index: false, redirect: false }));
app.use((req, res, next) => {
  angularApp.handle(req).then((response) => response ? writeResponseToNodeResponse(response, res) : next()).catch(next);
});

if (isMainModule(import.meta.url)) {
  const port = process.env['PORT'] || 4000;
  app.listen(port, () => console.log(`Angular SSR server listening on http://localhost:${port}`));
}

export const reqHandler = createNodeRequestHandler(app);
