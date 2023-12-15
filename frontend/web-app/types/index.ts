export type PagedResut<T> = {
  results: T[]
  pageNumber: number
  pageSize: number
  pageCount: number
  totalCount: number
}

export * from './auctions'
export * from './bids'