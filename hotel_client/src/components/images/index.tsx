import { SetStateAction, useState } from "react"
import { IUserModule } from "../../module/iUserModule"
import { ChevronRightIcon, XMarkIcon } from "@heroicons/react/24/outline"
import ImageHolder from "../imageHolder"
import { generalMessage } from "../../util/generalPrint"
import { iImageHolder } from "../../module/IImageHolder"
import { ChevronLeftIcon } from "@heroicons/react/16/solid"

interface iUserProps {
    images: iImageHolder[] | undefined,
    setImagesToNull: React.Dispatch<SetStateAction<iImageHolder[] | undefined>>

}

const ImagesPlay = ({ images = undefined, setImagesToNull }: iUserProps) => {
 
    const [index, setIndex] = useState(0)
    return <div className={`fixed  bg-gray-600  bottom-14   flex flex-row   h-screen w-screen top-0 start-0  transition-opacity duration-300 ${images ? 'opacity-100  z-30' : 'opacity-0  -z-10'}`}>
        <button
            onClick={(e) => {
                        generalMessage(`this shown the button is cliecked`)

                e.preventDefault()
                setImagesToNull(undefined)
            }}
            style={{ cursor: 'pointer' }}
            className='absolute z-30'
        >
            <XMarkIcon className='h-7 w-6 text-white absolute top-0' />

        </button>

        {
            images && <div className="relative h-[calc(100%-30px)]  w-screen mt-[30px] flex flex-row justify-center items-center">
                <button
                    onClick={(e) => {
                        e.preventDefault()
                        if (index + 1 < images.length) {
                            setIndex(index + 1)
                        }
                    }}
                    className="absolute end-1">
                    <ChevronRightIcon className={`h-6 w-6 ${index + 1 < images.length?' text-white':'text-gray-400'}`} />
                </button>
                <div>
                    <ImageHolder
                        src={images === undefined ? undefined : `${import.meta.env.VITE_MINIO_ENDPOINT}/room/${images[index].path}`}
                        style='flex flex-row h-24 w-24 '
                        isFromTop={true} />
                </div>

                <button
                    onClick={(e) => {
                        e.preventDefault()
                        if (index - 1 != -1) {
                            setIndex(index - 1)
                        }
                    }}
                    className="absolute start-1">
                    <ChevronLeftIcon className={`h-6 w-6 ${index - 1 != -1?' text-white':'text-gray-400'}`} />
                </button>
            </div>
        }
    </div>
}

export default ImagesPlay;