export const TenantStatus = [
    "Chưa kích hoạt",
    "Ngừng hoạt động",
    "Đang hoạt động",
]

export const ThesisStatus = [
    "Chờ phê duyệt",
    "Chờ xác nhận đề tài",
    "Từ chối hướng dẫn",
    "Đã xác nhận đề tài",
    "Đã hoàn thành"
]

export const ThesisStatusEnum = {
    WaitingForApproval: 0,
    ApprovedGuiding: 1,
    RejectGuiding: 2,
    ApprovedTitle: 3,
    Finished: 4,
}

export const TenantStatusEnum = {
    NotActive: 0,
    Stop: 1,
    Active: 2,
}

export const EntityStateEnum = {
    None : 0,
    Create : 1,
    Update : 2,
    Delete : 3
}