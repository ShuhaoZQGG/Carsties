import React from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResut } from '@/types';
async function getData(): Promise<PagedResut<Auction>> {
  const res = await fetch('http://localhost:6001/search?pageSize=10');

  if (!res.ok) throw new Error('Failed to fetch data');

  return res.json();
}

export default async function Navbar() {
  const data = await getData();
  return (
    <div className='grid grid-cols-4 gap-6'>
      { data && data.results.map((auction: Auction) => (
        <AuctionCard key={auction.id} auction={auction}/>
      )) }
    </div>
  )
}
