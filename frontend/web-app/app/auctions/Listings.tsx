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
export default function Navbar() {
  const [data, setData] = useState<PagedResut<Auction>>();
  const params = useParamsStore(state => ({
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy: state.orderBy,
    filterBy: state.filterBy
  }), shallow);
  const setParams = useParamsStore(state => state.setParams);
  const url = qs.stringifyUrl({ url: '', query: params })

  const setPageNumber = (pageNumber: number) => {
    setParams({pageNumber});
  }
  useEffect(() => {
    getData(url).then(data => {
      setData(data)
    });
  }, [url, setData])
  if (!data) return <h3>Loading...</h3>
  return (
    <>
      <Filters/>
      { data.totalCount === 0 ? (
        <EmptyFilter showReset/>
      ) : (
        <>
          <div className='grid grid-cols-4 gap-6'>
          { data.results.map((auction: Auction) => (
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
