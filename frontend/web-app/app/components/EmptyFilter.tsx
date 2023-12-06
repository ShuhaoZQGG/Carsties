import { useParamsStore } from '@/hooks/useParamsStore'
import React from 'react'
import Heading from './Heading'
import { Button } from 'flowbite-react'

type Props = {
  title?: string
  subTitle?: string
  showReset?: boolean
}

export default function EmptyFilter({
  title = 'No Matches for this filter', 
  subTitle = 'Try changing for resetting the filter', 
  showReset
} : Props) {
  const reset = useParamsStore(state => state.reset);
  return (
    <div className='h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg'>
      <Heading title={title} subTitle={subTitle} center/>
      <div className='mt-4'>
        { showReset && (
          <Button outline onClick={ reset }>Remove Filters</Button>
        )}
      </div> 
    </div>
  )
}
