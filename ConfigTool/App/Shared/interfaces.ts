//TypeScript models that matches the API’s ViewModels

export interface IUser {
    id: number;
    name: string;
    avatar: string;
    profession: string;
    schedulesCreated: number;
}

export interface ISchedule {
    id: number;
    title: string;
    description: string;
    timeStart: Date;
    timeEnd: Date;
    location: string;
    type: string;
    status: string;
    dateCreated: Date;
    dateUpdated: Date;
    creator: string;
    creatorId: number;
    attendees: number[];
}

export interface IScheduleDetails {
    id: number;
    title: string;
    description: string;
    timeStart: Date;
    timeEnd: Date;
    location: string;
    type: string;
    status: string;
    dateCreated: Date;
    dateUpdated: Date;
    creator: string;
    creatorId: number;
    attendees: IUser[];
    statuses: string[];
    types: string[];
}

export interface Pagination {
    CurrentPage: number;
    ItemsPerPage: number;
    TotalItems: number;
    TotalPages: number;
}

export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}

//Predicate interface is a predicate which allows us to pass generic predicates in TypeScript functions
export interface Predicate<T> {
    (item: T): boolean
}

//Assuming that you have an array of type IUser and you want to remove any user item that has id< 0 you would write:

//    this.itemsService.removeItems<IUser>(this.users, x => x.id < 0);