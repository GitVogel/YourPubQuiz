import {EnumInformation} from "../helpers/enum-information";

export enum Type {
  MultipleChoice = "multiple",
  TrueFalse = "boolean"
}

export const AllTypes: EnumInformation<Type>[] =[
  {
    enumValue: Type.MultipleChoice,
    userFriendlyName: "Multiple choice"
  },
  {
    enumValue: Type.TrueFalse,
    userFriendlyName: "True / false"
  }
]
