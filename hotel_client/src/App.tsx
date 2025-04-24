import { BrowserRouter, Route, Routes } from "react-router-dom";
import NotFoundPage from "./pages/NotFound/notfound";
import PrivateRout from "./services/privateRout";
import Home from "./pages/home";
import Login from "./pages/login";
import SignUp from "./pages/signUp";
import {
    QueryClient,
    QueryClientProvider,
} from '@tanstack/react-query'
import AuthRoute from "./services/AuthRoute";
import User from "./pages/user/user";
import RoomType from "./pages/roomType/roomType";
import Room from "./pages/room";


const App = () => {
    const queryClient = new QueryClient()

    return (
        <QueryClientProvider client={queryClient}>

            <BrowserRouter>
                <Routes>
                    <Route path="/" element={
                        <PrivateRout Page={Home} />
                    } />

                    <Route path="/users" element={
                        <PrivateRout Page={User} />
                    } />

                    <Route path="/roomType" element={
                        <PrivateRout Page={RoomType} />
                    } />

                    <Route path="/room" element={
                        <PrivateRout Page={Room} />
                    } />

                    <Route path='/login' element={
                        <AuthRoute Page={Login}
                        />
                    } />

                    <Route path='/signup' element={
                        <AuthRoute Page={SignUp} />
                    } />
                    <Route path='*' element={<NotFoundPage />} />
                </Routes>
            </BrowserRouter>
        </QueryClientProvider>

    )
}
export default App;