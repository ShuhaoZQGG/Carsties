'use client'
import React, {useState} from 'react'
import { Pagination } from 'flowbite-react'
type Props = {
  currentPage: number
  totalPages: number
  pageChanged: (page: number) => void
}
export default function AddPagination({ currentPage, totalPages, pageChanged }: Props) {
  const [pageNumber, setPageNumber] = useState(currentPage);
  return (
    <Pagination
      currentPage={currentPage}
      onPageChange={e => pageChanged(e)}
      totalPages={totalPages}
      layout='pagination'
      showIcons={true}
      className='text-blue-500 mb-5'
    />
  )
}
