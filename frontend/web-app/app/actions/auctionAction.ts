'use server'
import { Auction, PagedResut } from "@/types";

export async function getData(query: string): Promise<PagedResut<Auction>> {
  const res = await fetch(`http://localhost:6001/search${query}`);

  if (!res.ok) throw new Error('Failed to fetch data');

  return res.json();
}