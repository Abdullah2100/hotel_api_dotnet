import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface iAuthState{
    token?:string|null,
    refreshToken?:string|null
}

const initialState:iAuthState = {
    token:localStorage.getItem('access_client'),
   // token:'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MjY4YTQ3MC1hNzk2LTQxNzEtODM4Yi02ZTYxMzZlODM5ZGEiLCJzdWIiOiJkZThiZjcwMC03MzU5LTRhODAtYWE2OC03MjRmYTI0ZWNlZWEiLCJlbWFpbCI6ImFzZGFAZ21haWwuY29tIiwibmJmIjoxNzM0OTQ1NzcyLCJleHAiOjE3Mzc1Mzc3NzIsImlhdCI6MTczNDk0NTc3MiwiaXNzIjoiYTFiMmMzZDRlNWY2ZzdoOGk5ajBrMWwybTNuNG81cDYtYTFiMmMzZDRlNWY2ZzdoOGk5ajBrMWwybTNuNG81cDYiLCJhdWQiOiJhMWIyYzNkNGU1ZjZnN2g4aTlqLTBrMWwybTNuNG81cDZhMWIyYzNkNGU1ZjZnN2g4LWk5ajBrMWwybTNuNG81cDYifQ.oS2ZwytoFEGeaYECPT0BBZhqLIu4NDSmBkfX7S6UCB8',
    refreshToken:localStorage.getItem('reffresh_client')
   // refreshToken:'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MjY4YTQ3MC1hNzk2LTQxNzEtODM4Yi02ZTYxMzZlODM5ZGEiLCJzdWIiOiJkZThiZjcwMC03MzU5LTRhODAtYWE2OC03MjRmYTI0ZWNlZWEiLCJlbWFpbCI6ImFzZGFAZ21haWwuY29tIiwibmJmIjoxNzM0OTQ1NzcyLCJleHAiOjE3Mzc1Mzc3NzIsImlhdCI6MTczNDk0NTc3MiwiaXNzIjoiYTFiMmMzZDRlNWY2ZzdoOGk5ajBrMWwybTNuNG81cDYtYTFiMmMzZDRlNWY2ZzdoOGk5ajBrMWwybTNuNG81cDYiLCJhdWQiOiJhMWIyYzNkNGU1ZjZnN2g4aTlqLTBrMWwybTNuNG81cDZhMWIyYzNkNGU1ZjZnN2g4LWk5ajBrMWwybTNuNG81cDYifQ.oS2ZwytoFEGeaYECPT0BBZhqLIu4NDSmBkfX7S6UCB8'
}

const jwtSlice = createSlice({
    name: 'auth',
    initialState,  // Fix the typo here
    reducers: {
      // Action to set tokens
      setTokens: (state, action: PayloadAction<{ accessToken: string|undefined; refreshToken: string|undefined}>) => {
        state.token = action.payload.accessToken;
        state.refreshToken = action.payload.refreshToken;
        if(action.payload.accessToken!=undefined)
        localStorage.setItem('access_client', action.payload.accessToken);
      if(action.payload.refreshToken!=undefined)
           localStorage.setItem('reffresh_client', action.payload.refreshToken);
      },
      logout:(state)=>{
        state.token = null;
        state.refreshToken = null;
        localStorage.removeItem('access_client');
        localStorage.removeItem('reffresh_client');
      }
    },
  });


  export const { setTokens ,logout } = jwtSlice.actions;  // Export the action creator
  export default jwtSlice.reducer;  