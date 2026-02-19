import {EnumInformation} from "../helpers/enum-information";

export enum Difficulty {
  Easy = "easy",
  Medium = "medium",
  Hard = "hard"
}

export const AllDifficulties: EnumInformation<Difficulty>[] =[
  {
    enumValue: Difficulty.Easy,
    userFriendlyName: "Easy"
  },
  {
    enumValue: Difficulty.Medium,
    userFriendlyName: "Medium"
  },
  {
    enumValue: Difficulty.Hard,
    userFriendlyName: "Hard"
  }
]
