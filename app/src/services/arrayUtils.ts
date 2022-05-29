import { KeyValue } from '@angular/common';

export type Lookup<T = string> = { [key: string]: T };
export type HashTable = { lookup: Lookup; inverse: Lookup };

export const order =
  <T, U>(property: (obj: T) => U, order: 'Asc' | 'Desc' = 'Desc') =>
  (a: T, b: T) => {
    if (order === 'Asc') return property(a) < property(b) ? -1 : 1;
    return property(a) > property(b) ? -1 : 1;
  };

export const groupBy = <T>(
  list: T[],
  property: (obj: T) => string
): Lookup<T[]> => {
  const lookup: Lookup<T[]> = {};

  for (let item of list) {
    lookup[property(item)] = [...(lookup[property(item)] || []), item];
  }

  return lookup;
};

export const keyValueSort =
  (order: 'Asc' | 'Desc' = 'Desc') =>
  (ka: KeyValue<string, unknown>, kb: KeyValue<string, unknown>): number => {
    if (order === 'Asc') return ka.key < kb.key ? -1 : 1;
    return ka.key > kb.key ? -1 : 1;
  };

export const lookupSort =
  (lookup: Lookup<number>, order: 'Asc' | 'Desc' = 'Desc') =>
  (ka: KeyValue<string, unknown>, kb: KeyValue<string, unknown>): number => {
    if (order === 'Asc')
      return lookup[ka.key || 'Default'] < lookup[kb.key || 'Default'] ? -1 : 1;
    return lookup[ka.key || 'Default'] > lookup[kb.key || 'Default'] ? -1 : 1;
  };
