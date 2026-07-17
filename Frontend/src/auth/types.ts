export type LoginRequest = {
    email: string;
    password: string;
};

export type LoginResponse = {
    accessToken: string;
    expireAt: string;
};

export type CurrentMember = {
    id: number;
    name: string;
    email: string;
    role: string;
};