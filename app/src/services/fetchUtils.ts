import { catchError, Observable, switchMap } from 'rxjs';

export function HandleRequest<T>() {
  return (source: Observable<Response>) => {
    return source.pipe(
      switchMap((r) => {
        if (!r.ok) throw new Error(`Error ${r.status}`);
        return r.json() as Promise<T>;
      }),
      catchError((err) => {
        console.error(err);
        throw new Error(err.message);
      })
    );
  };
}
