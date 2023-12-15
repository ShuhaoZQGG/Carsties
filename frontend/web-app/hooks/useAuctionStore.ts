import { Auction, PagedResut } from "@/types"
import { create } from "zustand"

type State = {
  auctions: Auction[],
  totalCount: number,
  pageCount: number,
}

type Actions = {
  setData: (data: PagedResut<Auction>) => void,
  setCurrentPrice: (auctionId: string, amount: number) => void
}

const initialState : State = {
  auctions: [],
  totalCount: 0,
  pageCount: 0
}

export const useAuctionStore = create<State & Actions>((set) =>({
  ...initialState,
  setData: (data: PagedResut<Auction>) => {
    set(() => ({
      auctions: data.results,
      totalCount: data.totalCount,
      pageCount: data.pageCount
    }))
  },
  setCurrentPrice: (auctionId: string, amount: number) => {
    set((state) => ({
      auctions: state.auctions.map((auction) => auction.id === auctionId 
                ? { ...auction, currentHightBid: amount }
                : auction)
    }))
  }
}))