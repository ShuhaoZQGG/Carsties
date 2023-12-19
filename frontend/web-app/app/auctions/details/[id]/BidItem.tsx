import { Bid } from '@/types'
import React from 'react'
import { format } from 'date-fns';
import { numberWithCommas } from '@/app/lib/numberWithComma';
type Props = {
  bid: Bid
}

export default function BidItem({ bid }: Props) {
  function getBidInfo() {
    let bgColor = '';
    let text = '';
    switch(bid.bidStatus) {
      case 'Accepted':
        bgColor = 'bg-green-200'
        text = 'Bid accepted'
      break;
      case 'AcceptedBelowReserve':
        bgColor = 'bg-amber-500'
        text = 'Resrve not met'
        break;
      case 'TooLow':
        bgColor = 'bg-red-200'
        text = 'Bid was too low'
        break;
      default:
        bgColor = 'bg-red-200'
        text = 'Bid placed after auction finished'
        break;
    }
    return { bgColor, text }
  }
  const { bgColor, text } = getBidInfo();
  return (
    <div className={`border-gray-300 border-2 px-3 py-2 rounded-lg flex justify-between items-center mb-2 ${bgColor}`}>
      <div className='flex flex-col'>
        <span>Bidder: { bid.bidder }</span>
        <span className='text-gray-700 text-sm'>Time: { format(new Date(bid.bidTime), 'MMMM dd yyyy E hh:mm:ss') }</span>
      </div>
      <div className='flext flex-col right'>
        <div className='text-xl font-semibold'>${numberWithCommas(bid.amount)}</div>
        <div className='flex flex-row items-center'>
          <span>{text}</span>
        </div>
      </div>
    </div>
  )
}
