'use client'
import React from 'react'
import { Button, Dropdown } from 'flowbite-react'
import { User } from 'next-auth'
import { signOut } from 'next-auth/react'
import Link from 'next/link'
import { HiCog, HiUser } from 'react-icons/hi2'
import { AiFillTrophy, AiOutlineLogout } from 'react-icons/ai'
type Props = {
  user: Partial<User>
}

export default function UserActions({ user }: Props) {
  return (
  <Dropdown label={`Welcome ${user.name}`} inline>
    <Dropdown.Item icon={HiUser}>
      <Link href='/'>
        My Auctions
      </Link>
    </Dropdown.Item>
    <Dropdown.Item icon={ AiFillTrophy }>
      <Link href='/'>
        Auctions won
      </Link>
    </Dropdown.Item>
    <Dropdown.Item icon={HiCog}>
      <Link href='/session'>
        Session (dev only)
      </Link>
    </Dropdown.Item>
    <Dropdown.Divider/>
    <Dropdown.Item icon={ AiOutlineLogout } onClick={() => signOut({callbackUrl: '/'})}>
      Sign out
    </Dropdown.Item>
  </Dropdown>

  )
}
