import { UserCircleIcon, XMarkIcon } from '@heroicons/react/16/solid'
import React, { useState } from 'react';
import { generalMessage } from '../../util/generalPrint';
import { enNavLinkType } from '../../module/enNavLinkType';
import { PhotoIcon } from '@heroicons/react/24/outline';

interface ImageHolderProps {
  src?: string | undefined;
  style?: string | undefined;
  iconColor?:string |undefined;
  isFromTop?: boolean | undefined;
  typeHolder?: enNavLinkType | undefined;
  deleteFun?: () => void | undefined;
}

const ImageHolder = ({
  src,
  style,
  iconColor,
  isFromTop = false,
  typeHolder = undefined,
  deleteFun = undefined
}: ImageHolderProps) => {
  if(deleteFun===undefined)
   generalMessage(`\n\nthis the src file ${src}\n\n`)
  const [isHasError, setHasError] = useState<boolean>(false);

  const handleError = (e) => {
    setHasError(true);
  };

  const handleNotFoundIconHolder = () => {

    switch (typeHolder) {

      case enNavLinkType.ROOMS: return <PhotoIcon />
      default: return <UserCircleIcon className={`${iconColor ? 'text-white' : ''}`} />
    }

  }
  const imageHandler = () => {

    return (src === undefined || isHasError) ? handleNotFoundIconHolder() : <img
      
      src={src} onError={handleError} />;
  };

  return (
    <div className={`${style} relative`}>
      {
        deleteFun &&
        <button onClick={deleteFun} className='bg-white group hover:bg-gray-400 cursor-pointer absolute end-1 rounded-sm top-1'>
        <XMarkIcon 
          className={` h-6 w-6  hover:text-gray-50  `} />
        </button>
      }
      {imageHandler()}
    </div>
  );
};

export default ImageHolder;
