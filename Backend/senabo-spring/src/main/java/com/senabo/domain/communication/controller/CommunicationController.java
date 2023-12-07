package com.senabo.domain.communication.controller;

import com.senabo.common.ActivityType;
import com.senabo.common.api.ApiResponse;
import com.senabo.domain.affection.service.AffectionService;
import com.senabo.domain.communication.dto.response.CommunicationResponse;
import com.senabo.domain.communication.entity.Communication;
import com.senabo.domain.communication.service.CommunicationService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.service.StressService;
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
@RequestMapping("/communication")
@Tag(name = "Communication", description = "Communication API Document")
public class CommunicationController {

    private final CommunicationService communicationService;
    private final ReportService reportService;
    private final MemberService memberService;
    private final AffectionService affectionService;
    private final StressService stressService;

    @PostMapping("/save/{type}")
    @Operation(summary = "교감 내역 저장", description = "교감 내역을 저장한다. WAIT(기다려), SIT(앉아), HAND(손), PAT(쓰다듬기), TUG(터그놀이) 5가지 타입이 있다.")
    public ApiResponse<CommunicationResponse> createCommunication(@AuthenticationPrincipal UserDetails principal, @PathVariable ActivityType type) {
        if (type == ActivityType.WALK) return ApiResponse.fail("WALK(산책)는 저장 할 수 없습니다.", null);
        Member member = memberService.findByEmail(principal.getUsername());
        int changeAmount = 5;
        CommunicationResponse response = communicationService.createCommunication(member, type);
        affectionService.saveAffection(member, type, changeAmount);
        stressService.saveStress(member, StressType.COMMUNICATION, -1);
        return ApiResponse.success("교감 내역 저장 성공", response);
    }


    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = CommunicationResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "교감 전체 조회", description = "교감 내역을 전체 조회한다.")
    public ApiResponse<List<CommunicationResponse>> getCommunication(@AuthenticationPrincipal UserDetails principal) {
        List<Communication> communication = communicationService.getCommunication(principal.getUsername());
        if (communication.isEmpty()) return ApiResponse.fail("교감 전체 조회 실패", null);
        List<CommunicationResponse> response = communication.stream()
                .map(CommunicationResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("교감 전체 조회 성공", response);
    }

    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = CommunicationResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "교감 주간 조회", description = "교감 내역을 주간 조회한다.")
    public ApiResponse<List<CommunicationResponse>> getCommunication(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional =  reportService.findReportWeek(member, week);
        if(reportOptional.isEmpty())  return ApiResponse.fail("교감 "+ week + "주차 조회 실패", null);
        List<Communication> communication = communicationService.getCommunicationWeek(reportOptional.get(), member);
        List<CommunicationResponse> response = communication.stream()
                .map(CommunicationResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("교감 " + week + "주차 조회 성공", response);
    }


    @DeleteMapping("/remove")
    @Operation(summary = "교감 내역 삭제", description = "교감 내역을 삭제한다.")
    public ApiResponse<Object> removeCommunication(@AuthenticationPrincipal UserDetails principal) {
        communicationService.removeCommunication(principal.getUsername());
        return ApiResponse.success("교감 삭제 성공", true);
    }

}
