package com.senabo.domain.report.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.report.dto.request.UpdateTotalTimeRequest;
import com.senabo.domain.report.dto.response.ReportResponse;
import com.senabo.domain.report.dto.response.SimpleReportResponse;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/report")
@Tag(name = "Report", description = "Report API Document")
public class ReportController {

    private final ReportService reportService;

    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = SimpleReportResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "주간 리포트 전체 조회", description = "주간 리포트 내역을 전체 조회한다.")
    public ApiResponse<List<SimpleReportResponse>> getReport(@AuthenticationPrincipal UserDetails principal) {
        List<Report> report = reportService.getReport(principal.getUsername());
        if (report.isEmpty()) return ApiResponse.fail("주간 리포트 전체 조회 실패", null);
        List<SimpleReportResponse> response = report.stream()
                .map(SimpleReportResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("주간 리포트 전체 조회 성공", response);
    }

    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = ReportResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "주간 리포트 주간 조회", description = "주간 리포트 내역을 주간 조회한다.")
    public ApiResponse<ReportResponse> getReportWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Optional<Report> response = reportService.getReportWeek(principal.getUsername(), week);
        if (response.isEmpty()) return ApiResponse.fail("주간 리포트 " + week + "주차 조회 실패", null);
        return ApiResponse.success("주간 리포트 " + week + "주차 조회 성공", ReportResponse.from(response.get()));
    }

//    @PutMapping("/time")
//    @Operation(summary = "총 사용 시간 업데이트", description = "처음 접속 시간과 마지막 접속 시간을 받아서 총 사용 시간을 업데이트한다.")
//    public ApiResponse<ReportResponse> updateTotalTimeReport(@AuthenticationPrincipal UserDetails principal, @RequestBody UpdateTotalTimeRequest request) {
//        ReportResponse response = reportService.updateTotalTime(principal.getUsername(), request);
//        return ApiResponse.success("총 사용 시간 업데이트 성공", response);
//    }
}
