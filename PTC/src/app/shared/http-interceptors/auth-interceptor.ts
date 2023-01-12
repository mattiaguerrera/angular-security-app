import { Injectable } from '@angular/core';
import { HttpClient, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    let auth = undefined;
    let value = localStorage.getItem("AuthObject");
    if (value) {
      auth = JSON.parse(value);
    }

    if (auth) {
      // Clone the request and replace the original headers with
      // cloned headers, updated with the authorization.
      const authReq = req.clone({
        headers: req.headers.set('Authorization',
          'Bearer ' + auth.bearerToken)
      });

      // send cloned request with header to the next handler.
      return next.handle(authReq);
    }
    else {
      return next.handle(req);
    }
  }
}
