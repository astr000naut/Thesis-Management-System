
<template>
    <div class="page__container flex-col rg-2">
        <PopupUpload
            v-model:visible="popupUpload.visible"
            pUrlUpload="/worker/api/students/upload"
            pUrlDownloadSample="/worker/api/students/sample_upload_file"
            @close="popupUploadOnClose"
        ></PopupUpload>
        <StudentDetail
            v-model:visible="popupDetail.visible"
            :pEntityId="popupDetail.entityId"
            :pMode="popupDetail.mode"
        />
        <div class="page__header flex-row">
            <h1 class="page__title" style="font-size: 24px">
                Danh sách Sinh viên
            </h1>
            <el-button type="primary" @click="btnImportOnClick"
                >Nhập khẩu</el-button
            >
        </div>
        <div class="search-container flex-row al-center cg-2">
            <div class="flex-left">
                <el-select
                v-model="selectedFaculty"
                placeholder="Chọn khoa"
                size="large"
                style="width: 240px"
                @change="facultyOnChanged"
                >
                <el-option
                    v-for="item in facultyOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                />
            </el-select>
            </div>
            <div class="flex-right flex-row cg-2">
                <div class="reload-btn">
                    <el-button :icon="Refresh" circle @click="btnRefreshOnClick" />
                </div>
                <div class="search-input">
                    <el-input
                        v-model="searchText"
                        style="width: 240px"
                        placeholder="Tìm kiếm"
                        :prefix-icon="Search"
                        @input="searchTextOnInput"
                        clearable
                    />
                </div>
            </div>
        </div>
        <div class="table__container fl-1">
            <el-table
                :data="entities"
                style="width: 100%"
                row-key="userId"
                v-loading="loading"
            >
                <el-table-column
                    fixed
                    prop="studentCode"
                    label="Mã sinh viên"
                    width="150"
                />
                <el-table-column prop="studentName" label="Tên sinh viên" width="300" />
                <el-table-column prop="facultyName" label="Khoa" width="300" />
                <el-table-column
                    prop="major"
                    label="Chuyên ngành"
                    width="200"
                />
                <el-table-column prop="class" label="Lớp" width="120" />
                <el-table-column prop="gpa" label="GPA" width="120" />
                <el-table-column prop="email" label="Email" width="200" />
                <el-table-column
                    prop="phoneNumber"
                    label="Số điện thoại"
                    width="120"
                />
                <el-table-column fixed="right" label="Thao tác" width="120">
                    <template #default="scope">
                        <el-dropdown
                            size="small"
                            split-button
                            type="default"
                            @click="btnViewItemOnClick(scope.row)"
                        >
                            Xem
                            <template #dropdown>
                                <el-dropdown-menu>
                                    <el-dropdown-item                                      
                                        @click="btnEditItemOnClick(scope.row)"
                                        >Sửa</el-dropdown-item                                  
                                    >
                                    <el-dropdown-item        
                                        @click="btnDeleteItemOnClick(scope.row)"
                                        >Xóa</el-dropdown-item
                                    >
                                </el-dropdown-menu>
                            </template>
                        </el-dropdown>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div class="pagination">
            <el-pagination
                :current-page="pageNumber"
                :page-size="pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :small="false"
                :disabled="loading"
                :background="false"
                layout="total, sizes, prev, pager, next"
                :total="total"
                @size-change="studentStore.setPageSize"
                @current-change="studentStore.setPageNumber"
            />
        </div>
    </div>
</template>


<script setup>
import { onMounted, ref } from "vue";
import { useStudentStore, useFacultyStore } from "@/stores";
import { useRouter } from "vue-router";
import { storeToRefs } from "pinia";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { debounce } from "lodash";
import PopupUpload from '@/components/common/PopupUpload.vue';
import StudentDetail from "./StudentDetail.vue";

const router = useRouter();
const studentStore = useStudentStore();
const facultyStore = useFacultyStore();

const { entities, total, loading, keySearch, pageNumber, pageSize } =
    storeToRefs(studentStore);

const {entities: facultyEntities} = storeToRefs(facultyStore);

let debouncedFunction = null;

const facultyOptions = ref([
    { value: "-1", label: "Tất cả khoa" },
]);
const selectedFaculty = ref("-1");

const searchText = ref("");
const popupUpload = ref({
    visible: false,
});

const popupDetail = ref({
    visible: false,
    entityId: null,
    mode: "view",
});

initData();



async function initData() {

    await studentStore.fetchList();

    facultyStore.setPageSize(0);
    await facultyStore.fetchList();
    facultyOptions.value = facultyOptions.value.concat(
        facultyEntities.value.map((item) => ({
            value: item.facultyCode,
            label: item.facultyName,
        }))
    );
}

async function facultyOnChanged(value) {
    const customWhere = [
        {
            command: 'AND',
            columnName: 'facultyCode',
            operator: '=',
            value: value
        }
    ];

    if (value === '-1') {
        customWhere.pop();
    }

    await studentStore.fetchList(customWhere);
}

async function popupUploadOnClose(isUploadSuccess) {
    if (isUploadSuccess) {
        await studentStore.fetchList();
    }
}

const btnImportOnClick = () => {
    popupUpload.value.visible = true;
};

const btnDeleteItemOnClick = (row) => {
    ElMessageBox.confirm(
        `Bạn có chắc chắn muốn xóa ${row.studentCode}-${row.studentName} ?`,
        "Xác nhận",
        {
            confirmButtonText: "Đồng ý",
            cancelButtonText: "Hủy",
            type: "warning",
        }
    ).then(async () => {
        const isDeleted = await studentStore.deleteOne(row.userId);
        console.log(isDeleted);
        if (isDeleted) {
            ElMessage.success("Xóa thành công");
        } else {
            // ElMessage.error('Xóa thất bại');
        }
    });
};

async function searchTextOnInput() {
    if (!debouncedFunction) {
        debouncedFunction = debounce(() => {
            studentStore.setKeySearch(searchText.value);
        }, 800);
    }
    debouncedFunction();
}

const btnViewItemOnClick = (row) => {
    popupDetail.value = {
        visible: true,
        entityId: row['userId'],
        mode: "view",
    };
};

const btnEditItemOnClick = (row) => {
    popupDetail.value = {
        visible: true,
        entityId: row['userId'],
        mode: "edit",
    };
};

async function btnRefreshOnClick() {
    await studentStore.fetchList();
}

</script>

<style scoped>
.page__container {
    height: 100%;
    position: relative;
}

.el-table {
    color: #000;
    height: 100%;
}

.page__header {
    justify-content: space-between;
    align-items: center;
}

.table__container {
    overflow: auto;
}

.pagination {
    border-radius: 4px;
    display: flex;
    justify-content: flex-end;
    background-color: #fff;
    padding: 4px 10px 4px 10px;
}

:deep(.el-pagination__total) {
    position: absolute;
    left: 10px;
    font-weight: bold;
}

.search-container {
    justify-content: space-between;
}

:deep(.el-button.is-circle) {
    width: 32px;
    height: 32px !important;
}

:deep(.el-dropdown__caret-button) {
    outline: unset;
}

:deep(.el-dropdown__caret-button:active) {
    border-left: 1px solid #409eff;
}
</style>
