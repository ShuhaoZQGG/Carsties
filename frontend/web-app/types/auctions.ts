export type Auction = {
  id: string
  reservePrice: number
  winner?: string
  seller: string
  soldAmount: number
  currentHighBid: number
  model: string
  make: string
  imageUrl: string
  year: number
  auctionEnd: Date
  createdAt: string
  updatedAt: string
  color: string
  mileage: number
  status: string
}