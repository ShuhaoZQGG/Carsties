'use client'
import { useAuctionStore } from '@/hooks/useAuctionStore'
import { useBidStore } from '@/hooks/useBidStore'
import { Bid } from '@/types'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import React, { ReactNode, useEffect, useState } from 'react'

type Props = {
  children: ReactNode
}

export default function SignalRProvider({ children }: Props) {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);
  const addBid = useBidStore(state => state.addBid);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
                            .withUrl('http://localhost:6001/notifications')
                            .withAutomaticReconnect()
                            .build();
    setConnection(newConnection);
  }, [])

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log("Connected to notificaiton hub");
          connection.on('BidPlaced', (bid: Bid) => {
            console.log('BidPlaced Event Received:');
            console.log(bid);
            console.log(bid.bidStatus);
            console.log(bid.bidStatus.includes('Accepted'));
            if (bid.bidStatus.includes('Accepted')) {
              console.log("setting current price");
              setCurrentPrice(bid.auctionId, bid.amount);
            }
            addBid(bid);
          })
        })
        .catch(err => console.log(err));
    }

    return () => {
      connection?.stop();
    }
  }, [connection, setCurrentPrice, addBid])

  return (
    children
  )
}
