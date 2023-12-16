'use server'
import { Auction, Bid, PagedResut } from "@/types";
import { fetchWrapper } from "@/lib/fetchWrapper";
import { FieldValues } from "react-hook-form";
import { revalidatePath } from "next/cache";
export async function getData(query: string): Promise<PagedResut<Auction>> {
  return await fetchWrapper.get(`search${query}`)
}

export async function updateAuctionTest() {
  const data = {
    mileage: Math.floor(Math.random() * 10000) + 1
  }

  return await fetchWrapper.put('auctions/dc1e4071-d19d-459b-b848-b5c3cd3d151f', data);   
}

export async function createAuction(data: FieldValues) {
  return await fetchWrapper.post('auctions', data)
}

export async function getDetailedViewData(id: string) {
  return await fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(id: string, data: FieldValues) {
  const res = await fetchWrapper.put(`auctions/${id}`, data)
    // after update car, invalidate cache so can see the changed value immediately
    revalidatePath(`auctions/${id}`);
    return res;
}

export async function deleteAuction(id: string) {
  return await fetchWrapper.del(`auctions/${id}`);
}

export async function getBidsForAuction(id: string): Promise<Bid[]> {
  return await fetchWrapper.get(`bids/${id}`);
}

export async function placeBidForAuction(auctionId: string, amount: number) {
  return await fetchWrapper.post(`bids?auctionId=${auctionId}&amount=${amount}`, {});
}