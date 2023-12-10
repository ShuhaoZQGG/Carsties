'use client';

import { deleteAuction } from '@/app/actions/auctionAction';
import { Button } from 'flowbite-react';
import { useRouter } from 'next/navigation';
import React, { useState } from 'react'
import { toast } from 'react-hot-toast';

type Props = {
    id: string
}

export default function DeleteButton({id}: Props) {
    const [loading, setLoading] = useState(false);
    const router = useRouter();

    function doDelete() {
        setLoading(true);
        deleteAuction(id)
            .then((res: { error: any; }) => {
                if (res.error) throw res.error;
                router.push('/');
            }).catch((error: { status: string; message: string; }) => {
                toast.error(error.status + ' ' + error.message)
            }).finally(() => setLoading(false)) 
    }

    return (
        <Button color='failure' isProcessing={loading} onClick={doDelete}>
            Delete Auction
        </Button>
    )
}