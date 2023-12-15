'use client'
import React, { useState, useEffect } from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResut } from '@/types';
import AddPagination from '../components/AddPagination';
import { getData } from '../actions/auctionAction';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/useParamsStore';
import { shallow } from 'zustand/shallow';
import qs from 'query-string';
import EmptyFilter from '../components/EmptyFilter';
import { useAuctionStore } from '@/hooks/useAuctionStore';
export default function Navbar() {
  const [loading, setLoading] = useState(true);

  const params = useParamsStore(state => ({
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy,
    seller: state.seller,
    winner: state.winner
  }), shallow);
  const setParams = useParamsStore(state => state.setParams);

  const data = useAuctionStore(state => ({
    auctions: state.auctions,
    totalCount: state.totalCount,
    pageCount: state.pageCount,
  }), shallow);
  const setData = useAuctionStore(state => state.setData);

  const url = qs.stringifyUrl({ url: '', query: params })

  const setPageNumber = (pageNumber: number) => {
    setParams({pageNumber});
  }
  useEffect(() => {
    getData(url).then(data => {
      setData(data)
      setLoading(false);
    });
  }, [url, setData])
  if (loading) return <h3>Loading...</h3>
  return (
    <>
      <Filters/>
      { data.totalCount === 0 ? (
        <EmptyFilter showReset/>
      ) : (
        <>
          <div className='grid grid-cols-4 gap-6'>
          { data.auctions.map((auction: Auction) => (
            <AuctionCard key={auction.id} auction={auction}/>
          )) }
          </div>
          <div>
            <AddPagination pageChanged={setPageNumber} currentPage={params.pageNumber} totalPages={data.pageCount}/>
          </div>
        </>
      )}
    </>
  )
}
