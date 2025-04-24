// rootReducer.ts
import { combineReducers } from 'redux';
import jwtSlice from './redux/jwtSlice';  // Import the correct default export

// Combine the reducers
const rootReducer = combineReducers({
  auth: jwtSlice  // Use the reducer directly here
});

export default rootReducer;

// Define the RootState type for type inference
export type RootState = ReturnType<typeof rootReducer>;
