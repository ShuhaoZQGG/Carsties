'use client'
import React, { useState, useEffect } from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResut } from '@/types';
import AddPagination from '../components/AddPagination';
import { getData } from '../actions/auctionAction';

export default function Navbar() {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [pageCount, setPageCount] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() => {
    getData(pageNumber).then(data => {
      setAuctions(data.results);
      setPageCount(data.pageCount);
    });
  }, [pageNumber])
  if (auctions.length === 0) return <h3>Loading...</h3>
  return (
    <>
      <div className='grid grid-cols-4 gap-6'>
        { auctions && auctions.map((auction: Auction) => (
          <AuctionCard key={auction.id} auction={auction}/>
        )) }
      </div>
      <div>
        <AddPagination pageChanged={setPageNumber} currentPage={pageNumber} totalPages={pageCount}/>
      </div>
    </>
  )
}
