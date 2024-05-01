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