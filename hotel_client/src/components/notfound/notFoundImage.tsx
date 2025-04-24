import 'react'
import './styleNotFound.css';
import notFoundImage from '../../assets/page-not-found.png'

const NotFoundImage = ()=>{
return (
    <>
       <img src={notFoundImage} className={"notFoundImage"}/>
    </>
)
}

export default NotFoundImage;